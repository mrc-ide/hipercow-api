// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Prometheus;

    /// <summary>
    /// The implementation of IMetrics.
    /// </summary>
    public class MetricsUpdateService : BackgroundService
    {
        private readonly IClusterHandleCache clusterHandleCache;
        private Dictionary<string, Dictionary<string, dynamic>> userJobs = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsUpdateService"/> class.
        /// </summary>
        /// <param name="clusterHandleCache">
        /// Handle cache so we can query the headnode.
        /// </param>
        public MetricsUpdateService(IClusterHandleCache clusterHandleCache)
        {
            this.clusterHandleCache = clusterHandleCache;
        }

        /// <summary>
        /// The async task to periodically update the metrics.
        /// </summary>
        /// <param name="stoppingToken">
        /// Token for aborting the periodic updates when shutting down.</param>
        /// <returns>I am not sure.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<string> clusters = DideConstants.GetDideClusters();
                foreach (var cluster in clusters)
                {
                    this.userJobs.Clear();
                    this.UpdateByState(cluster, JobState.Running, int.MaxValue);
                    this.UpdateByState(cluster, JobState.Queued, int.MaxValue);
                    this.UpdateByState(cluster, JobState.Finished, 24);
                    this.UpdateByState(cluster, JobState.Failed, 24);
                    this.UpdateByState(cluster, JobState.Canceled, 24);

                    foreach (var user in this.userJobs.Keys)
                    {
                        var details = this.userJobs[user];
                        foreach (string state in new List<string> { "Running", "Queued", "Finished", "Failed", "Cancelled" })
                        {
                            MetricsRegistry.JobsGauge.WithLabels([cluster, user, state]).Set(details[state]);
                        }

                        MetricsRegistry.CoreHoursGauge.WithLabels([cluster, user]).Set(details["coreHours"]);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private static float GetCoreHours(PropertyRow job, JobState state)
        {
            var start = (DateTime)job[JobPropertyIds.StartTime].Value;
            var end = (state == JobState.Finished) ? (DateTime)job[JobPropertyIds.EndTime].Value : DateTime.Now;
            var unitType = (JobUnitType)job[JobPropertyIds.UnitType].Value;
            var unitCount = (int)job[JobPropertyIds.MinCores].Value;
            var noCores = (unitType == JobUnitType.Core) ? unitCount : unitCount * 32;
            TimeSpan span = end - start;
            return (float)span.TotalHours * noCores;
        }

        private void UpdateByState(string cluster, JobState state, int maxHoursAgo)
        {
            IScheduler scheduler = this.clusterHandleCache.GetClusterHandle(cluster)!;

            PropertyIdCollection props = [JobPropertyIds.Id, JobPropertyIds.UserName, JobPropertyIds.Owner, JobPropertyIds.ChangeTime,
            JobPropertyIds.StartTime, JobPropertyIds.EndTime, JobPropertyIds.MinCores, JobPropertyIds.UnitType];

            IFilterCollection jobFilter = scheduler.CreateFilterCollection();
            jobFilter.Add(FilterOperator.Equal, JobPropertyIds.State, state);
            ISortCollection sortFilter = new SortCollection
            {
                {
                    SortProperty.SortOrder.Descending,
                    JobPropertyIds.ChangeTime
                },
            };

            ISchedulerRowEnumerator jobs = scheduler.OpenJobEnumerator(props, jobFilter, sortFilter);
            var now = DateTime.Now;
            var stateName = (state == JobState.Canceled) ? "Cancelled" : Enum.GetName(state)!;
            foreach (var job in jobs)
            {
                var changeTime = job[JobPropertyIds.ChangeTime];
                TimeSpan diff = now - (DateTime)changeTime.Value;

                if (diff.TotalHours >= maxHoursAgo)
                {
                    break;
                }

                string user = Utils.HPCString(job[JobPropertyIds.UserName]);
                string owner = Utils.HPCString(job[JobPropertyIds.Owner]);
                user = (user.Trim() == string.Empty) ? owner : user;
                user = user.Replace("DIDE\\", string.Empty);

                this.userJobs.TryGetValue(user, out Dictionary<string, dynamic>? value);
                if (value is null)
                {
                    value = [];
                    value["Finished"] = 0;
                    value["Queued"] = 0;
                    value["Cancelled"] = 0;
                    value["Running"] = 0;
                    value["Failed"] = 0;
                    value["coreHours"] = 0.0f;
                    this.userJobs.Add(user, value);
                }

                value[stateName]++;
                if ((state == JobState.Finished) || (state == JobState.Running))
                {
                    value["coreHours"] += GetCoreHours(job, state);
                }
            }
        }
    }
}

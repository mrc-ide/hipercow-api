// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// The implementation of IMetrics.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MetricsUpdateService"/> class.
    /// </remarks>
    /// <param name="clusterHandleCache">
    /// Handle cache so we can query the headnode.
    /// </param>
    public class MetricsUpdateService(
        IClusterHandleCache clusterHandleCache) : BackgroundService
    {
        private readonly IClusterHandleCache clusterHandleCache = clusterHandleCache;
        private readonly Dictionary<string, Dictionary<string, dynamic>> userJobs = [];

        /// <summary>
        /// Calculate core hours for a Finished or Running job.
        /// </summary>
        /// <param name="job">PropertyRow for the job, returned from the
        /// ISchedulerJobEnumerator.</param>
        /// <param name="state">HPC JobState for the job.
        /// Since I deal with states separately, I don't query this back
        /// from the cluster, so it's not in the job property, to keep that
        /// query as light as possible. Perhaps pedantic.</param>
        /// <returns>The number of core hours, as a float.</returns>
        internal static float GetCoreHours(PropertyRow job, JobState state)
        {
            var start = (DateTime)job[JobPropertyIds.StartTime].Value;
            var end = (state == JobState.Finished) ?
                (DateTime)job[JobPropertyIds.EndTime].Value :
                DateTime.Now;
            var unitType = (JobUnitType)job[JobPropertyIds.UnitType].Value;
            var unitCount = (unitType == JobUnitType.Core) ?
                (int)job[JobPropertyIds.MinCores].Value :
                (int)job[JobPropertyIds.MinNodes].Value;
            var noCores = (unitType == JobUnitType.Core) ?
                unitCount :
                unitCount * 32;
            TimeSpan span = end - start;
            return (float)span.TotalHours * noCores;
        }

        /// <summary>
        /// Return user jobs for testing.
        /// </summary>
        /// <returns>The userJobs structure.
        /// </returns>
        internal Dictionary<string, Dictionary<string, dynamic>> GetUserJobs()
        {
            return this.userJobs;
        }
        
        /// <summary>
        /// Query a cluster and update the userJobs records, which store
        /// for each user, how many jobs they have (in recent history), in
        /// a particular state, and also how many core-hours they have
        /// consumed. I made this state-specific as there will be different
        /// maxHoursAgo settings for different states, and the querying can
        /// be made quicker if it is state specific.
        /// </summary>
        /// <param name="cluster">Name of the headnode to query.</param>
        /// <param name="state">Include only jobs in this state.</param>
        /// <param name="maxHoursAgo">Include only jobs whose status
        /// changed within this window.</param>
        internal void UpdateByState(string cluster, JobState state, int maxHoursAgo)
        {
            IScheduler scheduler = this.clusterHandleCache.GetClusterHandle(cluster)!;

            PropertyIdCollection props = [
                JobPropertyIds.UserName, JobPropertyIds.Owner, JobPropertyIds.ChangeTime,
                JobPropertyIds.StartTime, JobPropertyIds.EndTime, JobPropertyIds.MinCores,
                JobPropertyIds.MinNodes, JobPropertyIds.UnitType];

            IFilterCollection jobFilter = scheduler.CreateFilterCollection();
            jobFilter.Add(FilterOperator.Equal, JobPropertyIds.State, state);
            ISortCollection sortFilter = new SortCollection
            {
                {
                    SortProperty.SortOrder.Descending,
                    JobPropertyIds.ChangeTime
                },
            };

            ISchedulerRowEnumerator jobs = scheduler.OpenJobEnumerator(
                props, jobFilter, sortFilter);
            var now = DateTime.Now;
            var stateName = (state == JobState.Canceled) ? "Cancelled" : Enum.GetName(state)!;
            var jobList = jobs.GetRows(int.MaxValue);

            foreach (var job in jobList.Rows)
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

        /// <summary>
        /// The async task to periodically update the metrics.
        /// </summary>
        /// <param name="stoppingToken">
        /// Token for aborting the periodic updates.</param>
        /// <returns>An async task.</returns>
        // Unsure how to test this function - will test others...
        [ExcludeFromCodeCoverage]
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
                        foreach (string state in new List<string>
                        {
                            "Running", "Queued", "Finished", "Failed", "Cancelled",
                        })
                        {
                            MetricsRegistry.JobsGauge.
                                WithLabels([cluster, user, state]).
                                Set(details[state]);
                        }

                        MetricsRegistry.CoreHoursGauge.
                            WithLabels([cluster, user]).
                            Set(details["coreHours"]);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}

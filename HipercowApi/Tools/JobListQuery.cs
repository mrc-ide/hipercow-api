// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// The implementation of IJobListQuery, used to
    /// query a cluster headnode for job information.
    /// </summary>
    public class JobListQuery : IJobListQuery
    {
        /// <inheritdoc/>
        public JobList GetJobList(
            string cluster,
            IScheduler scheduler,
            string? user,
            string? state,
            int maxRows = 500)
        {
            var jobs = new List<JobInfo>();
            JobState? hpcstate = Utils.HPCJobState(state);
            IFilterCollection jobFilter = scheduler.CreateFilterCollection();
            AddIfNotNull(jobFilter, FilterOperator.Equal, PropId.Job_UserName, user);
            AddIfNotNull(jobFilter, FilterOperator.Equal, PropId.Job_State, hpcstate);

            PropertyIdCollection props =
            [
                JobPropertyIds.Id,
                JobPropertyIds.UserName,
                JobPropertyIds.Name,
                JobPropertyIds.State,
            ];

            var rowEnum = scheduler.OpenJobEnumerator(props, jobFilter, null);
            var jobList = rowEnum.GetRows(maxRows);

            foreach (PropertyRow job in jobList.Rows)
            {
                jobs.Add(new JobInfo(
                    Utils.HPCInt(job[JobPropertyIds.Id]),
                    Utils.HPCString(job[JobPropertyIds.Name]),
                    Utils.HPCString(job[JobPropertyIds.State])));
            }

            return new JobList(jobs);
        }

        private static void AddIfNotNull(
            IFilterCollection jf,
            FilterOperator fo,
            PropId pi,
            object? val)
        {
            if (val is not null)
            {
                jf.Add(fo, pi, val);
            }
        }
    }
}

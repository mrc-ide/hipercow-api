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
            int? maxRows)
        {
            var jobs = new List<JobInfo>();
            IFilterCollection jobFilter = scheduler.CreateFilterCollection();

            // state is either null, or a valid job state (string) at this point.
            // The non-null invalid state is dealt with in the controller.
            if (state is not null)
            {
                jobFilter.Add(FilterOperator.Equal, PropId.Job_State, Utils.HPCJobState(state));
            }

            if (user is not null)
            {
                jobFilter.Add(FilterOperator.Equal, PropId.Job_UserName, user);
            }

            PropertyIdCollection props =
            [
                JobPropertyIds.Id,
                JobPropertyIds.UserName,
                JobPropertyIds.Name,
                JobPropertyIds.State,
            ];

            var rowEnum = scheduler.OpenJobEnumerator(props, jobFilter, null);
            var jobList = rowEnum.GetRows(maxRows ?? 500);

            foreach (PropertyRow job in jobList.Rows)
            {
                jobs.Add(new JobInfo(
                    Utils.HPCInt(job[JobPropertyIds.Id]),
                    Utils.HPCString(job[JobPropertyIds.Name]),
                    Utils.HPCString(job[JobPropertyIds.State])));
            }

            return new JobList(jobs);
        }
    }
}

// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Wrapper for MSHPC ISchedulerCore, so we can
    /// test cluster load without needing a real cluster.
    /// </summary>
    public interface IHipercowSchedulerCore
    {
        /// <summary>
        /// Return the state of the core.
        /// </summary>
        /// <returns>The HPC state of the core.</returns>
        public SchedulerCoreState GetState();
    }
}

// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Wrapper for MSHPC ISchedulerCore, so we can
    /// test cluster load without needing a real cluster.
    /// </summary>
    public interface IHipercowSchedulerNode
    {
        /// <summary>
        /// Return the cores that this node has, with current states.
        /// </summary>
        /// <returns>A Collection of core objects.</returns>
        public ISchedulerCollection GetCores();
    }
}

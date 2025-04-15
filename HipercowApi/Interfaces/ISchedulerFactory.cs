// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Factory for returning a new Scheduler object.
    /// </summary>
    public interface ISchedulerFactory
    {
        /// <summary>
        /// Return a new Scheduler object.
        /// </summary>
        /// <returns>A new Scheduler object.</returns>
        public IScheduler NewScheduler();
    }
}

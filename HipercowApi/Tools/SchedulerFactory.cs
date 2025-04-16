// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Factory for creating HPC scheduler objects.
    /// </summary>
    public class SchedulerFactory : ISchedulerFactory
    {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public IScheduler NewScheduler()
        {
            return new Scheduler();
        }
    }
}

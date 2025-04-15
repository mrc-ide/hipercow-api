// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Miscellaneous support helper functions to make the code more readable elsewhere.
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

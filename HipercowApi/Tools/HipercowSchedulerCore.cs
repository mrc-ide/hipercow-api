// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Wrapper for MS HPC Scheduler, so we can test without needing a real cluster.
    /// Because genuine calls will require talking to a real head-node, this class
    /// is excluded from code coverage, and will consist of minimal wrapping.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HipercowSchedulerCore : IHipercowSchedulerCore
    {
        private SchedulerCoreState testState = SchedulerCoreState.Reserved;
        private ISchedulerCore? hpcCore = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HipercowSchedulerCore"/> class,
        /// which either wraps an ISchedulerCore, or sets the testState that we'll
        /// return instead.
        /// </summary>
        /// <param name="hpcCore">If not null, wrap this HPC ISchedulerCore.</param>
        /// <param name="testState">The state to return if we don't wrap an ISchedulerCore.</param>
        public HipercowSchedulerCore(ISchedulerCore? hpcCore, SchedulerCoreState testState = SchedulerCoreState.Reserved)
        {
            this.hpcCore = hpcCore;
            this.testState = testState;
        }

        /// <summary>
        /// Return the state of the node (or the test data if this is not a real node).
        /// </summary>
        /// <returns>The SchedulerCoreState - an enum including Offline, Idle, Busy, Draining, Reserved.</returns>
        public SchedulerCoreState GetState()
        {
            if (this.hpcCore is null)
            {
                return this.testState;
            }
            else
            {
                return this.hpcCore.State;
            }
        }
    }
}

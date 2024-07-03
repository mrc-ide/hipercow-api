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
    public class HipercowScheduler : IHipercowScheduler
    {
        private Scheduler? scheduler = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HipercowScheduler"/> class.
        /// </summary>
        public HipercowScheduler()
        {
            this.scheduler = new Scheduler();
        }

        /// <summary>
        /// Connect the scheduler object to a named cluster.
        /// </summary>
        /// <param name="cluster">Name of the cluster to connect to.</param>
        public void Connect(string cluster)
        {
            this.scheduler?.Connect(cluster);
        }

        /// <summary>
        /// Wrapper for IScheduler.OpenNodeEnumerator and GetRows, to
        /// return a list of rows queried from the cluster.
        /// </summary>
        /// <param name="properties">The properties to retrieve.</param>
        /// <param name="filter">The filter to use.</param>
        /// <param name="sorter">The sorting method to apply.</param>
        /// <returns>A property row set giving information about each node.</returns>
        public PropertyRowSet? NodesQuery(
            IPropertyIdCollection properties,
            IFilterCollection filter,
            ISortCollection sorter)
        {
            var nodeEnum = this.HipercowSchedulerOpenNodeEnumerator(
                properties, filter, sorter);

            return nodeEnum?.GetRows(int.MaxValue);
        }

        private ISchedulerRowEnumerator? HipercowSchedulerOpenNodeEnumerator(
            IPropertyIdCollection properties,
            IFilterCollection filter,
            ISortCollection sorter)
        {
            return this.scheduler?.OpenNodeEnumerator(properties, filter, sorter);
        }
    }
}

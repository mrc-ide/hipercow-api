// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Wrapper for MSHPC Scheduler, so we can
    /// test without needing a real cluster.
    /// </summary>
    public interface IHipercowScheduler
    {
        /// <summary>
        /// Connect the scheduler object to a named cluster.
        /// </summary>
        /// <param name="cluster">Name of the cluster to connect to.</param>
        public void Connect(string cluster);

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
            ISortCollection sorter);

        /// <summary>
        /// Wrapper for IScheduler.GetNodeList, to
        /// return a list of nodes and their current states.
        /// </summary>
        /// <param name="filter">The filter to use.</param>
        /// <param name="sorter">The sorting method to apply.</param>
        /// <returns>A ISchedulerCollection of ISchedulerNodes.</returns>
        public ISchedulerCollection GetNodeList(
            IFilterCollection? filter,
            ISortCollection? sorter);
    }
}

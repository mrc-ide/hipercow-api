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
        private SchedulerCollection<IHipercowSchedulerNode> testData = new SchedulerCollection<IHipercowSchedulerNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HipercowScheduler"/> class.
        /// </summary>
        public HipercowScheduler()
        {
            this.scheduler = new Scheduler();
        }

        /// <summary>
        /// Inject test data that getNodeList will return.
        /// </summary>
        /// <param name="data">A collection of HipercowSchedulerNode.</param>
        public void SetTestNodeList(SchedulerCollection<IHipercowSchedulerNode> data)
        {
            this.testData.Clear();
            this.testData = data;
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

        /// <summary>
        /// Wrapper for IScheduler.GetNodeList, to
        /// return a list of nodes, and their current states.
        /// </summary>
        /// <param name="filter">The filter to use.</param>
        /// <param name="sorter">The sorting method to apply.</param>
        /// <returns>A ISchedulerCollection of ISchedulerNodes.</returns>
        public ISchedulerCollection GetNodeList(
            IFilterCollection? filter = null,
            ISortCollection? sorter = null)
        {
            SchedulerCollection<HipercowSchedulerNode> res = new SchedulerCollection<HipercowSchedulerNode>();
            if (this.scheduler is null)
            {
                return res;
            }

            ISchedulerCollection hpcRes = this.scheduler.GetNodeList(filter, sorter);
            foreach (ISchedulerNode isn in hpcRes)
            {
                res.Add(new HipercowSchedulerNode(isn));
            }

            return res;
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

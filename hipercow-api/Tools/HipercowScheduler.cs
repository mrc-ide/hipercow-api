// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Wrapper for MSHPC Scheduler, so we can
    /// test without needing a real cluster.
    /// </summary>
    public class HipercowScheduler
    {
        private PropertyRowSet testData = new PropertyRowSet(null, null);
        private IScheduler? scheduler = null;
        private bool testing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HipercowScheduler"/> class.
        /// </summary>
        /// <param name="testing">Set to true if this is a virtual test cluster.</param>
        public HipercowScheduler(bool testing = false)
        {
            this.scheduler = (!testing) ? new Scheduler() : null;
            this.testing = testing;
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
        /// Populate testData placeholder, and return this from the
        /// NodesQuery call if testing.
        /// </summary>
        /// <param name="data">The PropertyRowSet of fake data.</param>
        public void SetTestData(PropertyRowSet data)
        {
            this.testData = data;
        }

        /// <summary>
        /// Wrapper for IScheduler.OpenNodeEnumerator and GetRows, to
        /// return a list of rows queried from the cluster.
        /// </summary>
        /// <param name="properties">The properties to retrieve.</param>
        /// <param name="filter">The filter to use.</param>
        /// <param name="sorter">The sorting method to apply.</param>
        /// <returns>A property row set giving information about each node.</returns>
        public PropertyRowSet NodesQuery(
            IPropertyIdCollection properties,
            IFilterCollection filter,
            ISortCollection sorter)
        {
            ISchedulerRowEnumerator? nodeEnum = (!this.testing) ?
                this.scheduler?.OpenNodeEnumerator(properties, filter, sorter) : null;

            return (nodeEnum != null) ?
                nodeEnum.GetRows(int.MaxValue) : this.testData;
        }
    }
}

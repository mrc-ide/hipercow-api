// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// The implementation of IClusterInfoQuery, used to
    /// query a cluster headnode for information.
    /// </summary>
    public class ClusterInfoQuery : IClusterInfoQuery
    {
        /// <inheritdoc/>
        public ClusterInfo GetClusterInfo(string cluster, IScheduler scheduler, IUtils utils)
        {
            var filter = Utils.GetFilterNonComputeNodes(cluster);
            var sorter = GetSorterAscending();
            var properties = GetNodeProperties();
            var rows = utils.NodesQuery(scheduler, properties, filter, sorter)!;

            var maxRam = (int)Math.Round((1 / 1024.0) * rows.Rows.Select(
                  (row) => Utils.HPCInt(row[NodePropertyIds.MemorySize])).Max());

            var maxCores = rows.Rows.Select(
                  (row) => Utils.HPCInt(row[NodePropertyIds.NumCores])).Max();

            var nodeNames = new List<string>(rows.Rows.Select(
                  (row) => Utils.HPCString(row[NodePropertyIds.Name])));

            return new ClusterInfo(
                cluster,
                maxRam,
                maxCores,
                nodeNames,
                DideConstants.GetQueues(cluster),
                DideConstants.GetDefaultQueue(cluster));
        }

        /// <summary>
        /// Helper to return a node sorter in alphabetical ascending order.
        /// </summary>
        /// <returns>A SortCollection object used for sorting in increasing node number.</returns>
        private static SortCollection GetSorterAscending()
        {
            return new SortCollection
            {
                {
                    SortProperty.SortOrder.Ascending,
                    NodePropertyIds.Name
                },
            };
        }

        /// <summary>
        /// Helper to return the set of properties we want to see when asking
        /// for information about a cluster.
        /// </summary>
        /// <returns>An ProprtyIdcollection including the node name, number of cores,
        /// and memory size, which we can query for.</returns>
        private static PropertyIdCollection GetNodeProperties()
        {
            return
            [
                NodePropertyIds.Name,
                NodePropertyIds.NumCores,
                NodePropertyIds.MemorySize,
            ];
        }
    }
}

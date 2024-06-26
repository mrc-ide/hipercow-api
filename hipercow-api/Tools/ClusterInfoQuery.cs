// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    using Hipercow_api.Models;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// The implementation of IClusterInfoQuery, used to
    /// query a cluster headnode for information.
    /// </summary>
    public class ClusterInfoQuery : IClusterInfoQuery
    {
        /// <inheritdoc/>
        public ClusterInfo? GetClusterInfo(string cluster, IHipercowScheduler? scheduler = null)
        {
            scheduler = scheduler ?? ClusterHandleCache.GetSingletonClusterHandleCache().GetClusterHandle(cluster)!;

            if (scheduler == null)
            {
                return null;
            }

            var filter = GetFilterNonComputeNodes(cluster);
            var sorter = GetSorterAscending();
            var properties = GetNodeProperties();
            var rows = scheduler.NodesQuery(properties, filter, sorter)!;

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
        /// Helper to return a search filter, which when used queries for all nodes except
        /// the head node.
        /// </summary>
        /// <param name="cluster">The cluster (headnode) name.</param>
        /// <returns>A FilterCollection object used for filtering.</returns>
        private static FilterCollection GetFilterNonComputeNodes(
            string cluster)
        {
            return new FilterCollection
            {
                {
                    FilterOperator.NotEqual,
                    PropId.Node_Name,
                    cluster
                },
            };
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

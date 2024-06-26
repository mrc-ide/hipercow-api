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
            if (scheduler == null)
            {
                scheduler = ClusterHandleCache.GetSingletonClusterHandleCache().GetClusterHandle(cluster)!;
            }

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
        /// <returns>An IFilterCollection object used for filtering.</returns>
        private static IFilterCollection GetFilterNonComputeNodes(
            string cluster)
        {
            var nodeFilter = new FilterCollection();

            nodeFilter.Add(
                FilterOperator.NotEqual,
                PropId.Node_Name,
                cluster);

            return nodeFilter;
        }

        /// <summary>
        /// Helper to return a node sorter in alphabetical ascending order.
        /// </summary>
        /// <returns>An ISortCollection object used for sorting in increasing node number.</returns>
        private static ISortCollection GetSorterAscending()
        {
            var nodeSorter = new SortCollection();

            nodeSorter.Add(
                SortProperty.SortOrder.Ascending,
                NodePropertyIds.Name);

            return nodeSorter;
        }

        /// <summary>
        /// Helper to return the set of properties we want to see when asking
        /// for information about a cluster.
        /// </summary>
        /// <returns>An IProprtyIdcollection including the node name, number of cores,
        /// and memory size, which we can query for.</returns>
        private static IPropertyIdCollection GetNodeProperties()
        {
            return new PropertyIdCollection()
            {
                NodePropertyIds.Name,
                NodePropertyIds.NumCores,
                NodePropertyIds.MemorySize,
            };
        }
    }
}

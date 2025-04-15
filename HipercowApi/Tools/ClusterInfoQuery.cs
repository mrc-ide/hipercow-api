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
        public ClusterInfo GetClusterInfo(string cluster, IScheduler scheduler)
        {
            var filter = Utils.GetFilterNonComputeNodes(cluster);
            var sorter = Utils.GetSorterAscending();
            var properties = Utils.GetNodeProperties();
            var rows = Utils.NodesQuery(scheduler, properties, filter, sorter)!;

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
    }
}

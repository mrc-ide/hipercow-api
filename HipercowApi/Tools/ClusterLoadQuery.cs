// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// The implementation of IClusterLoadQuery, used to
    /// query a cluster headnode for its current load.
    /// </summary>
    public class ClusterLoadQuery : IClusterLoadQuery
    {
        /// <inheritdoc/>
        public ClusterLoad GetClusterLoad(string cluster, IScheduler scheduler)
        {
            var nodeLoads = new List<NodeLoad>();
            var filter = Utils.GetExcludeNonComputeNodesFilter(cluster);
            var nodeList = scheduler.GetNodeList(filter, null);

            foreach (ISchedulerNode node in nodeList)
            {
                var cores = node.GetCores();
                int busy = 0;
                foreach (ISchedulerCore core in cores)
                {
                    busy += (core.State == SchedulerCoreState.Busy) ? 1 : 0;
                }

                nodeLoads.Add(new NodeLoad(
                    node.Name,
                    busy,
                    node.NumberOfCores,
                    node.State == NodeState.Online ? "Online" : "Offline"));
            }

            return new ClusterLoad(
                cluster,
                nodeLoads);
        }
    }
}

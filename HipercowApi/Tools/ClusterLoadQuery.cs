// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// The implementation of IClusterLoadQuery, used to
    /// query a cluster headnode for its current load.
    /// </summary>
    public class ClusterLoadQuery : IClusterLoadQuery
    {
        /// <inheritdoc/>
        public ClusterLoad? GetClusterLoad(string cluster, IHipercowScheduler? scheduler = null)
        {
            scheduler = scheduler ?? ClusterHandleCache.GetSingletonClusterHandleCache().GetClusterHandle(cluster)!;
            var nodeLoads = new List<NodeLoad>();
            if (scheduler == null)
            {
                return null;
            }

            var filter = Utils.GetFilterNonComputeNodes(cluster);
            var nodeList = scheduler.GetNodeList(filter, null);

            foreach (HipercowSchedulerNode node in nodeList)
            {
                var cores = node.GetCores();
                int busy = 0;

                foreach (HipercowSchedulerCore core in cores)
                {
                    busy += (core.GetState() == SchedulerCoreState.Busy) ? 1 : 0;
                }

                nodeLoads.Add(new NodeLoad(
                    node.GetName(),
                    busy,
                    node.GetNumCores(),
                    node.GetStateName(node.GetState())));
            }

            return new ClusterLoad(
                cluster,
                nodeLoads);
        }
    }
}

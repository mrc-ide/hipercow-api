// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    /// <summary>
    /// The HPC call to create the handle to a cluster can be slow. We want
    /// to cache these handles for re-use, and create them early to maximise
    /// performance. This class provides that functionality.
    /// </summary>
    public class ClusterHandle
    {
        /// <summary>
        /// A single cache of cluster handles - <see cref="ClusterHandleCache"/>.
        /// </summary>
        private static ClusterHandleCache clusterHandleCache = new ClusterHandleCache();

        /// <summary>
        /// Return the cluster handle cache - used for testing so we can
        /// fake a cluster node.
        /// </summary>
        /// <returns>The ClusterHandleCache.</returns>
        public static ClusterHandleCache GetClusterHandleCache()
        {
            return clusterHandleCache;
        }

        /// <summary>
        /// Return a handle to a named cluster, using the cache where possible,
        /// creating a new handle if it doesn't exist, or returning null if the
        /// cluster name was invalid.
        /// </summary>
        /// <param name="cluster">The name of the cluster.</param>
        /// <returns>A handle to that cluster, or null if the named cluster
        /// did not exist.</returns>
        public static HipercowScheduler? GetClusterHandle(string cluster)
        {
            HipercowScheduler? result;
            clusterHandleCache.TryGetValue(cluster, out result);
            if (result != null)
            {
                return result;
            }

            if (DideConstants.GetDideClusters().Contains(cluster))
            {
                HipercowScheduler scheduler = new HipercowScheduler();
                scheduler.Connect(cluster);
                clusterHandleCache.Add(cluster, scheduler);
                return scheduler;
            }

            return null;
        }
    }
}

// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    /// <summary>
    /// A dictionary of handles, so we can retrieve the scheduler
    /// handle for named cluster quickly, without having to recreate
    /// it every time (which is slow).
    /// </summary>
    public class ClusterHandleCache : Dictionary<string, HipercowScheduler>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterHandleCache"/> class.
        /// </summary>
        public ClusterHandleCache()
        {
            new Dictionary<string, HipercowScheduler>();
        }

        /// <summary>
        /// Initialise the dictionary of handles with a list of clusters.
        /// Called at the start to create all our handles in advance.
        /// </summary>
        /// <param name="clusters">List of clusters to get handles for.</param>
        /// <param name="testing">Testing mode - allows adding non-existent clusters.</param>
        public static void InitialiseHandles(List<string> clusters, bool testing = false)
        {
            foreach (string cluster in clusters)
            {
                ClusterHandle.GetClusterHandle(cluster, testing);
            }
        }
    }
}

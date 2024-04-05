// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// A dictionary of handles, so we can retrieve the IScheduler
    /// handle for named cluster quickly, without having to recreate
    /// it every time (which is slow).
    /// </summary>
    public class ClusterHandleCache : Dictionary<string, IScheduler>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterHandleCache"/> class.
        /// </summary>
        public ClusterHandleCache()
        {
            new Dictionary<string, IScheduler>();
        }

        /// <summary>
        /// Initialise the dictionary of handles with a list of clusters.
        /// Called at the start to create all our handles in advance.
        /// </summary>
        /// <param name="clusters">List of clusters to get handles for.</param>
        public static void InitialiseHandles(List<string> clusters)
        {
            clusters.Select((cluster) => ClusterHandle.GetClusterHandle(cluster));
        }
    }
}

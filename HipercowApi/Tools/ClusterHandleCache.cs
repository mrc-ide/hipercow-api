// Copyright (c) Imperial College London. All rights reserved.
namespace HipercowApi.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// A dictionary of handles, so we can retrieve the scheduler
    /// handle for named cluster quickly, without having to recreate
    /// it every time (which is slow).
    /// </summary>
    public class ClusterHandleCache : IClusterHandleCache
    {
        private Dictionary<string, IScheduler> handles;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterHandleCache"/> class.
        /// Only used in testing; in the app, call GetSingletonClusterHandleCache().
        /// </summary>
        public ClusterHandleCache()
        {
            this.handles = new Dictionary<string, IScheduler>();
        }

        /// <summary>
        /// Initialise the dictionary of handles with a list of clusters.
        /// Called at the start to create all our handles in advance.
        /// </summary>
        /// <param name="clusters">List of clusters to get handles for.</param>
        [ExcludeFromCodeCoverage]
        public void InitialiseHandles(List<string> clusters)
        {
            foreach (string cluster in clusters)
            {
                this.GetClusterHandle(cluster);
            }
        }

        /// <summary>
        /// Return a handle to a named cluster, using the cache where possible,
        /// creating a new handle if it doesn't exist, or returning null if the
        /// cluster name was invalid.
        /// </summary>
        /// <param name="cluster">The name of the cluster.</param>
        /// <returns>A handle to that cluster, or null if the named cluster does not exist.
        /// did not exist.</returns>
        [ExcludeFromCodeCoverage]
        public IScheduler? GetClusterHandle(string cluster)
        {
            List<string> dideClusters = DideConstants.GetDideClusters();
            IScheduler? result;
            this.handles.TryGetValue(cluster, out result);
            if (result != null)
            {
                return result;
            }

            if (dideClusters.Contains(cluster))
            {
                Scheduler scheduler = new Scheduler();
                scheduler.Connect(cluster);
                this.handles.Add(cluster, scheduler);
                return scheduler;
            }

            return null;
        }
    }
}

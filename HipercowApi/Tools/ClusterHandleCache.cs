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
    public class ClusterHandleCache : Dictionary<string, IHipercowScheduler>
    {
        private static ClusterHandleCache? singletonClusterHandleCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterHandleCache"/> class.
        /// Only used in testing; in the app, call GetSingletonClusterHandleCache().
        /// </summary>
        public ClusterHandleCache()
        {
            new Dictionary<string, IHipercowScheduler>();
        }

        /// <summary>
        /// Return the singleton cluster handle cache.
        /// </summary>
        /// <returns>The static cluster handle cache for the API.</returns>
        public static ClusterHandleCache GetSingletonClusterHandleCache()
        {
            if (singletonClusterHandleCache == null)
            {
                singletonClusterHandleCache = new ClusterHandleCache();
            }

            return singletonClusterHandleCache;
        }

        /// <summary>
        /// Initialise the dictionary of handles with a list of clusters.
        /// Called at the start to create all our handles in advance.
        /// </summary>
        /// <param name="clusters">List of clusters to get handles for.</param>
        /// <param name="dideClusters">For testing only - the list of acceptable cluster names.</param>
        /// <param name="scheduler">For testing only - a scheduler object for mocking.</param>
        public void InitialiseHandles(List<string> clusters, List<string>? dideClusters = null, IHipercowScheduler? scheduler = null)
        {
            foreach (string cluster in clusters)
            {
                this.GetClusterHandle(cluster, dideClusters, scheduler);
            }
        }

        /// <summary>
        /// Return a handle to a named cluster, using the cache where possible,
        /// creating a new handle if it doesn't exist, or returning null if the
        /// cluster name was invalid.
        /// </summary>
        /// <param name="cluster">The name of the cluster.</param>
        /// <param name="dideClusters">For testing only - the list of acceptable cluster names.</param>
        /// <param name="scheduler">For testing only - a scheduler object for mocking.</param>
        /// <returns>A handle to that cluster, or null if the named cluster does not exist.
        /// did not exist.</returns>
        public IHipercowScheduler? GetClusterHandle(string cluster, List<string>? dideClusters = null, IHipercowScheduler? scheduler = null)
        {
            dideClusters = dideClusters ?? DideConstants.GetDideClusters();
            IHipercowScheduler? result;
            this.TryGetValue(cluster, out result);
            if (result != null)
            {
                return result;
            }

            if (dideClusters.Contains(cluster))
            {
                scheduler = GetScheduler(scheduler);
                scheduler.Connect(cluster);
                this.Add(cluster, scheduler);
                return scheduler;
            }

            return null;
        }

        /// <summary>
        /// If the scheduler is null, then create a scheduler; this needs
        /// excluding from coverage testing, because it can only result
        /// in an attempt to access a real headnode.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private static IHipercowScheduler GetScheduler(IHipercowScheduler? scheduler = null)
        {
            return scheduler ?? new HipercowScheduler();
        }
    }
}

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
        private readonly Dictionary<string, IScheduler> handles;
        private readonly ISchedulerFactory schedulerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterHandleCache"/> class.
        /// </summary>
        /// <param name="schedulerFactory">Factory for creating the HPC scheduler object.</param>
        public ClusterHandleCache(ISchedulerFactory? schedulerFactory = null)
        {
            schedulerFactory = schedulerFactory ?? new SchedulerFactory();
            this.handles = new Dictionary<string, IScheduler>();
            this.schedulerFactory = schedulerFactory;
        }

        /// <summary>
        /// Initialise the dictionary of handles with a list of clusters.
        /// Called at the start to create all our handles in advance.
        /// </summary>
        /// <param name="clusters">List of clusters to get handles for.</param>
        [ExcludeFromCodeCoverage]
        public void InitialiseHandles(List<string> clusters)
        {
            clusters.ForEach(cluster => this.GetClusterHandle(cluster));
        }

        /// <inheritdoc/>
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
                IScheduler scheduler = this.schedulerFactory.NewScheduler();
                scheduler.Connect(cluster);
                this.handles.Add(cluster, scheduler);
                return scheduler;
            }

            return null;
        }
    }
}

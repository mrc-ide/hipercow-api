namespace Hipercow_api
{
    using Hipercow_api.Tools;
    using Microsoft.Hpc.Scheduler;

    public class ClusterHandle
    {
        private static ClusterHandleCache clusterHandleCache = new ClusterHandleCache();

        public static IScheduler GetClusterHandle(string cluster)
        {
            IScheduler? result;
            clusterHandleCache.TryGetValue(cluster, out result);
            if (result != null)
            {
                return result;
            }

            IScheduler scheduler = new Scheduler();
            scheduler.Connect(cluster);
            clusterHandleCache.Add(cluster, scheduler);
            return scheduler;
        }
    }
}

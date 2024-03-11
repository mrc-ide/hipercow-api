namespace Hipercow_api.Tools
{
    using Microsoft.Hpc.Scheduler;

    public class ClusterHandleCache : Dictionary<string, IScheduler>
    {
        public ClusterHandleCache()
        {
            new Dictionary<string, IScheduler>();
        }

        public static void InitialiseHandles(List<string> clusters)
        {
            clusters.Select((cluster) => ClusterHandle.GetClusterHandle(cluster));
        }
    }
}

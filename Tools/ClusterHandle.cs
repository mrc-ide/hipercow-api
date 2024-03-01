using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;
using static Microsoft.Hpc.Diagnostics.Helpers.StepResult;

namespace Hipercow_api
{
    public class ClusterHandle
    {
        private static Dictionary<string, IScheduler> clusterHandleCache =
            new Dictionary<string, IScheduler>();

        public static IScheduler? GetClusterHandle(string cluster)
        {
            if (clusterHandleCache.ContainsKey(cluster))
            {
                IScheduler result;
                clusterHandleCache.TryGetValue(cluster, out result);
                return result;
            }

            IScheduler scheduler = new Scheduler();
            scheduler.Connect(cluster);
            clusterHandleCache.Add(cluster, scheduler);
            return scheduler;
        }

        public static void InitialiseHandles(List<string> clusters)
        {
            clusters.Select((cluster) => GetClusterHandle(cluster));
        }
    }
}

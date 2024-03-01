using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;
using static Microsoft.Hpc.Diagnostics.Helpers.StepResult;

namespace hipercow_api
{
    public class ClusterHandle
    {
        static Dictionary<String, IScheduler> ClusterHandleCache =
            new Dictionary<String, IScheduler>();

        public static IScheduler? getClusterHandle(String cluster)
        {
            if (ClusterHandleCache.ContainsKey(cluster))
            {
                IScheduler result;
                ClusterHandleCache.TryGetValue(cluster, out result);
                return result;
            }

            IScheduler scheduler = new Scheduler();
            scheduler.Connect(cluster);
            ClusterHandleCache.Add(cluster, scheduler);
            return scheduler;
        }

        public static void InitialiseHandles(List<String> clusters)
        {
            clusters.Select((cluster) => getClusterHandle(cluster));
        }
    }
}

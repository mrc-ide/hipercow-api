namespace Hipercow_api
{
    using Hipercow_api.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    public class ClusterInfoQuery : IClusterInfoQuery
    {
        public ClusterInfo? GetClusterInfo(string cluster)
        {
            IScheduler? scheduler = ClusterHandle.GetClusterHandle(cluster);
            if (scheduler == null)
            {
                return null;
            }

            IFilterCollection filter = GetFilterNonComputeNodes(scheduler, cluster);
            ISortCollection sorter = GetSorterAscending(scheduler);
            IPropertyIdCollection properties = GetNodeProperties();
            ISchedulerCollection nodes = scheduler.GetNodeList(filter, sorter);
            ISchedulerRowEnumerator nodeEnumerator =
                scheduler.OpenNodeEnumerator(properties, filter, sorter);

            PropertyRowSet rows = nodeEnumerator.GetRows(int.MaxValue);

            int maxRam = (int)Math.Round((1 / 1024.0) * rows.Rows.Select(
                  (row) => Utils.HPCInt(row[NodePropertyIds.MemorySize])).Max());

            int maxCores = rows.Rows.Select(
                  (row) => Utils.HPCInt(row[NodePropertyIds.NumCores])).Max();

            List<string> nodeNames = new List<string>(rows.Rows.Select(
                  (row) => Utils.HPCString(row[NodePropertyIds.Name])));

            return new ClusterInfo(
                cluster,
                maxRam,
                maxCores,
                nodeNames,
                DideConstants.GetQueues(cluster),
                DideConstants.GetDefaultQueue(cluster));
        }

        private static IFilterCollection GetFilterNonComputeNodes(
            IScheduler scheduler,
            string cluster)
        {
            IFilterCollection nodeFilter =
                scheduler.CreateFilterCollection();

            nodeFilter.Add(
                FilterOperator.NotEqual,
                PropId.Node_Name,
                cluster);

            return nodeFilter;
        }

        private static ISortCollection GetSorterAscending(
            IScheduler scheduler)
        {
            ISortCollection nodeSorter =
               scheduler.CreateSortCollection();

            nodeSorter.Add(
                SortProperty.SortOrder.Ascending,
                NodePropertyIds.Name);

            return nodeSorter;
        }

        private static IPropertyIdCollection GetNodeProperties()
        {
            return new PropertyIdCollection()
            {
                NodePropertyIds.Name,
                NodePropertyIds.NumCores,
                NodePropertyIds.MemorySize
            };
        }
    }
}

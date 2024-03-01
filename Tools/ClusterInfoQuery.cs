namespace Hipercow_api
{
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    public static class ClusterInfoQuery
    {
        public static ClusterInfo GetClusterInfo(string cluster)
        {
            IScheduler scheduler = ClusterHandle.GetClusterHandle(cluster);
            ClusterInfo info = new ClusterInfo();

            IFilterCollection nodeFilter =
                scheduler.CreateFilterCollection();

            nodeFilter.Add(
                FilterOperator.NotEqual,
                PropId.Node_Name, 
                cluster);

            ISortCollection nodeSorter =
                scheduler.CreateSortCollection();

            nodeSorter.Add(
                SortProperty.SortOrder.Ascending,
                NodePropertyIds.Name);

            IPropertyIdCollection nodeProperties =
                new PropertyIdCollection()
            {
                NodePropertyIds.Name,
                NodePropertyIds.NumCores,
                NodePropertyIds.MemorySize
            };

            ISchedulerCollection nodes =
                scheduler.GetNodeList(nodeFilter, nodeSorter);

            ISchedulerRowEnumerator nodeEnumerator =
                scheduler.OpenNodeEnumerator(
                    nodeProperties,
                    nodeFilter,
                    nodeSorter);

            PropertyRowSet rows = nodeEnumerator.GetRows(999);

            info.Nodes = new List<string> (rows.Rows.Select(
                (row) => Utils.HPCString(row[NodePropertyIds.Name])));

            info.MaxRam = (int) Math.Round((1 / 1024.0) * rows.Rows.Select(
                (row) => Utils.HPCInt(row[NodePropertyIds.MemorySize])).Max());

            info.MaxCores = rows.Rows.Select(
                (row) => Utils.HPCInt(row[NodePropertyIds.NumCores])).Max();

            info.Name = cluster;

            info.Queues = DideConstants.GetQueues(cluster);

            info.DefaultQueue = DideConstants.GetDefaultQueue(cluster);

            return info;
        }
    }
}

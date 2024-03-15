namespace Hipercow_api
{
    public class ClusterInfo
    {
        public ClusterInfo(
            string name,
            int maxRam,
            int maxCores,
            List<string> nodes,
            List<string> queues,
            string defaultQueue)
        {
            this.Name = name;
            this.MaxRam = maxRam;
            this.MaxCores = maxCores;
            this.Nodes = nodes;
            this.Queues = queues;
            this.DefaultQueue = defaultQueue;
        }

        public string Name { get; set; }

        public int MaxRam { get; set; }

        public int MaxCores { get; set; }

        public List<string> Nodes { get; set; }

        public List<string> Queues { get; set; }

        public string DefaultQueue { get; set; }
    }
}

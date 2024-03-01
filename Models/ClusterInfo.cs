namespace Hipercow_api
{
    public class ClusterInfo
    {
        public string? Name { get; set; }

        public int MaxRam { get; set; }

        public int MaxCores { get; set; }

        public List<string>? Nodes { get; set; }

        public List<string>? Queues { get; set; }

        public string? DefaultQueue { get; set;  }
    }
}

namespace hipercow_api
{
    public class ClusterInfo
    {
        public String? Name { get; set; }
        public int MaxRam { get; set; }
        public int MaxCores { get; set; }
        public List<String>? Nodes { get; set; }
        public List<String>? Queues { get; set; }
        public String? DefaultQueue { get; set;  }
    }
}

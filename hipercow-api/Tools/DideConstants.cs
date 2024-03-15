namespace Hipercow_api
{
    public static class DideConstants
    {
        private static List<string> dideClusters = new List<string> 
        {
                "wpia-hn"
        };

        private static List<string> wpiaHnQueues = new List<string> 
        {
               "AllNodes",
               "Training"
        };

        public static List<string> GetDideClusters()
        {
            return dideClusters;
        }

        public static List<string> GetQueues(string cluster)
        {
            switch (cluster)
            {
                case "wpia-hn":
                    return wpiaHnQueues;
            }

            return new List<string>();
        }

        public static string GetDefaultQueue(string cluster)
        {
            List<string> queues = GetQueues(cluster);
            if (queues.Count > 0)
            {
                return queues[0];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

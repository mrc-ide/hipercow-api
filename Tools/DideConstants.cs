using Microsoft.Identity.Client;

namespace hipercow_api
{
    public static class DideConstants
    {
        public static List<String> DideClusters = new List<String> {
                "wpia-hn"
        };

        public static List<String> wpiahnQueues = new List<String> {
               "AllNodes",
               "Training"
        };

        public static List<String>? getQueues(String cluster)
        {
            switch (cluster)
            {
                case "wpia-hn":
                    return wpiahnQueues;
            }
            return null;
        }

        public static String getDefaultQueue(String cluster)
        {
            return getQueues(cluster)[0];
        }
    }
}

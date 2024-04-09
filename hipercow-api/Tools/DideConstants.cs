// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    /// <summary>
    /// A set of constants specific to our DIDE clusters. The intention is that
    /// these are not easy things to query, including metadata such as "which
    /// clusters do we actually have". Queues are also returned, since we may
    /// have queues for our internal use that we don't want the API to
    /// return.
    /// </summary>
    public static class DideConstants
    {
        /// <summary>
        /// The list of clusters we currently have. Which is just one, wpia-hn.
        /// </summary>
        private static List<string> dideClusters = new List<string>
        {
                "wpia-hn",
        };

        /// <summary>
        /// A list of clusters including a couple of fakes for testing.
        /// </summary>
        private static List<string> testClusters = new List<string>
        {
                "wpia-hn",
                "fake1",
                "fake2",
        };

        /// <summary>
        /// The list of queues that we publish for wpia-hn. If we add more
        /// clusters in the future, this should be come a look-up.
        /// </summary>
        private static List<string> wpiaHnQueues = new List<string>
        {
               "AllNodes",
               "Training",
        };

        /// <summary>
        /// Public function to return the list of published clusters.
        /// </summary>
        /// <param name="testing">If true, add two more clusters, fake1 and fake2.</param>
        /// <returns>A list of strings which are the cluster names.</returns>
        public static List<string> GetDideClusters(bool testing = false)
        {
            return (!testing) ? dideClusters : testClusters;
        }

        /// <summary>
        /// Return the published queues for a given cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster.</param>
        /// <returns>
        /// If the cluster is valid, a list of strings which are the names of possible queues. If the
        /// cluster is not valid, we return an empty list of strings.
        /// </returns>
        public static List<string> GetQueues(string cluster)
        {
            switch (cluster)
            {
                case "wpia-hn":
                    return wpiaHnQueues;
            }

            return new List<string>();
        }

        /// <summary>
        /// Return the default queue that jobs should be submitted to if no queue is explicitly specified.
        /// </summary>
        /// <param name="cluster">The name of the cluster.</param>
        /// <returns>
        /// The name of the default queue (ie, job template) for that cluster. If the cluster
        /// is invalid, an empty string is returned.
        /// </returns>
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

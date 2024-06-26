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
        /// The list of queues that we publish for wpia-hn. If we add more
        /// clusters in the future, this should be come a look-up.
        /// </summary>
        private static readonly List<string> WpiaHnQueues =
        [
               "AllNodes",
               "Training",
        ];

        /// <summary>
        /// Public function to return the list of published clusters.
        /// </summary>
        /// <returns>A list of strings which are the cluster names.</returns>
        public static List<string> GetDideClusters()
        {
            return ["wpia-hn"];
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
            return cluster switch
            {
                "wpia-hn" => WpiaHnQueues,
                _ => [],
            };
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
            var queues = GetQueues(cluster);
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

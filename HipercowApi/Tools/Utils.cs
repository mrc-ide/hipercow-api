// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Miscellaneous support helper functions to make the code more readable elsewhere.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return a string if
        /// possible, otherwise return an empty string, rather than null. In reality,
        /// a non-null StoreProperty can't have a null value or ToString - but I
        /// think it is not declared that way - hence code coverage disabled for
        /// this one-liner, as otherwise I get a partial line coverage that I can't
        /// complete.
        /// </summary>
        /// <param name="sp">A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when querying the cluster.</param>
        /// <returns>A string which either has a value or is empty, but is not null.</returns>
        [ExcludeFromCodeCoverage]
        public static string HPCString(StoreProperty sp)
        {
            return sp.Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return an int if
        /// possible. This is guaranteed to be an int for all valid usage.
        /// </summary>
        /// <param name="sp">A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when querying the cluster.</param>
        /// <returns>An integer value for the property.</returns>
        public static int HPCInt(StoreProperty sp)
        {
            return int.Parse(HPCString(sp));
        }

        /// <summary>
        /// Helper to return a search filter, which when used queries for all nodes except
        /// the head node.
        /// </summary>
        /// <param name="cluster">The cluster (headnode) name.</param>
        /// <returns>A FilterCollection object used for filtering.</returns>
        public static FilterCollection GetFilterNonComputeNodes(
            string cluster)
        {
            return new FilterCollection
            {
                {
                    FilterOperator.NotEqual,
                    PropId.Node_Name,
                    cluster
                },
            };
        }

        /// <summary>
        /// Wrapper for IScheduler.OpenNodeEnumerator and GetRows, to
        /// return a list of rows queried from the cluster.
        /// </summary>
        /// <param name="scheduler">The connected scheduler to query.</param>
        /// <param name="properties">The properties to retrieve.</param>
        /// <param name="filter">The filter to use.</param>
        /// <param name="sorter">The sorting method to apply.</param>
        /// <returns>A property row set giving information about each node.</returns>
        public static PropertyRowSet NodesQuery(
            IScheduler scheduler,
            IPropertyIdCollection properties,
            IFilterCollection filter,
            ISortCollection sorter)
        {
            var nodeEnum = scheduler.OpenNodeEnumerator(
                properties, filter, sorter);

            return nodeEnum.GetRows(int.MaxValue);
        }

        /// <summary>
        /// Helper to return a node sorter in alphabetical ascending order.
        /// </summary>
        /// <returns>A SortCollection object used for sorting in increasing node number.</returns>
        public static SortCollection GetSorterAscending()
        {
            return new SortCollection
            {
                {
                    SortProperty.SortOrder.Ascending,
                    NodePropertyIds.Name
                },
            };
        }

        /// <summary>
        /// Helper to return the set of properties we want to see when asking
        /// for information about a cluster.
        /// </summary>
        /// <returns>An ProprtyIdcollection including the node name, number of cores,
        /// and memory size, which we can query for.</returns>
        public static PropertyIdCollection GetNodeProperties()
        {
            return
            [
                NodePropertyIds.Name,
                NodePropertyIds.NumCores,
                NodePropertyIds.MemorySize,
            ];
        }
    }
}

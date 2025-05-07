// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.DirectoryServices.Protocols;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Miscellaneous support helper functions to make the code more
    /// readable elsewhere.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return a string
        /// if possible, otherwise return an empty string, rather than null.
        /// In reality, a non-null StoreProperty can't have a null value or
        /// ToString - but I think it is not declared that way - hence code coverage
        /// disabled for this one-liner, as otherwise I get a partial line coverage
        /// that I can't complete.
        /// </summary>
        /// <param name="sp">
        /// A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when
        /// querying the cluster.
        /// </param>
        /// <returns>
        /// A string which either has a value or is empty, but is not null.
        /// </returns>
        public static string HPCString(StoreProperty sp)
        {
            return sp.Value.ToString()!;
        }

        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return an int if
        /// possible. This is guaranteed to be an int for all valid usage.
        /// </summary>
        /// <param name="sp">
        /// A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when
        /// querying the cluster.
        /// </param>
        /// <returns>An integer value for the property.</returns>
        public static int HPCInt(StoreProperty sp)
        {
            return int.Parse(HPCString(sp));
        }

        /// <summary>
        /// Helper to return a search filter, which when used queries for all nodes
        /// except the head node.
        /// </summary>
        /// <param name="cluster">The cluster (headnode) name.</param>
        /// <returns>A FilterCollection object used for filtering.</returns>
        public static FilterCollection GetExcludeNonComputeNodesFilter(
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
        /// <returns>
        /// A SortCollection object used for sorting in increasing node number.
        /// </returns>
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

        /// <summary>
        /// Convert the name of a job state, to a JobState enum member.
        /// </summary>
        /// <param name="name">Name of the job state.</param>
        /// <returns>
        /// A JobState, or null if the name is not recognised or supported.
        /// </returns>
        public static JobState? HPCJobState(string name)
        {
            return Enum.TryParse(name, out JobState result) ? result : null;
        }

        /// <summary>
        /// Using LDAP, query and parse a list of groups that a DIDE
        /// domain user belongs to.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="ldap">A LdapConnection.</param>
        /// <returns>
        /// A list of names of groups.
        /// </returns>
        public static List<string> GetDomainGroups(string user, LdapConnection ldap)
        {
            List<string> groups = [];
            SearchRequest searchRequest = new(
            "OU=Users,OU=DIDE Users,DC=dide,DC=local",
            "(&(objectCategory=person)(SAMAccountName=" + user + "))",
            SearchScope.Subtree,
            new string[] { "SAMAccountName", "memberOf", "cn" });

            SearchResponse searchResponse = (SearchResponse)ldap.SendRequest(searchRequest);

            if (searchResponse.Entries.Count == 1)
            {
                SearchResultEntry item = searchResponse.Entries[0];
                for (int i = 0; i < item.Attributes["memberOf"].Count; i++)
                {
                    string result_part = item.Attributes["memberOf"][i].ToString()!;
                    string[] result_split = result_part.Split([',']);
                    for (int j = 0; j < (int)result_split.Length; j++)
                    {
                        if (result_split[j].StartsWith("CN"))
                        {
                            groups.Add(result_split[j].Substring(3));
                        }
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// A helper shared between controllers, for checking that the JWT is
        /// valid, the session ID is valid, and the users match between them.
        /// </summary>
        /// <param name="controller">The controller calling the test.</param>
        /// <param name="jwtUsername">The username from the JWT (possibly null).</param>
        /// <param name="session">The user session, possibly null.</param>
        /// <returns>
        /// An IActionResult? - an error-type code with message if a failure,
        /// otherwise null if no error was triggered.
        /// </returns>
        public static IActionResult? CheckTokenAndSession(ControllerBase controller, string? jwtUsername, UserSession? session)
        {
            if (string.IsNullOrEmpty(jwtUsername))
            {
                return controller.Unauthorized("Missing user identity from token");
            }

            if (session is null)
            {
                return controller.Unauthorized("Session expired or invalid.");
            }

            if (!string.Equals(session.Username, jwtUsername, StringComparison.OrdinalIgnoreCase))
            {
                return controller.Forbid("Session ID does not match the logged-in user.");
            }

            return null;
        }
    }
}

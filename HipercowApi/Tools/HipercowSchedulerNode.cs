// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Wrapper for MS HPC Scheduler, so we can test without needing a real cluster.
    /// Because genuine calls will require talking to a real head-node, this class
    /// is excluded from code coverage, and will consist of minimal wrapping.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HipercowSchedulerNode : IHipercowSchedulerNode
    {
        private SchedulerCollection<IHipercowSchedulerCore> testCores = new SchedulerCollection<IHipercowSchedulerCore>();
        private string testName;
        private int testNumCores;
        private NodeState testState;

        private ISchedulerNode? hpcNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HipercowSchedulerNode"/> class.
        /// </summary>
        /// <param name="hpcNode">If not null, then wrap an ISchedulerNode.</param>
        /// <param name="testCores">For testing: collection of cores to be returned by GetNodes if hpcNode is not specified.</param>
        /// <param name="testName">For testing: name of node if hpcNode is not specified.</param>
        /// <param name="testNumCores">For testing: number of cores on node if hpcNode is not specified.</param>
        /// <param name="testState">For testing: state of node if hpcNode is not specified.</param>
        public HipercowSchedulerNode(
            ISchedulerNode? hpcNode,
            SchedulerCollection<IHipercowSchedulerCore>? testCores = null,
            string testName = "",
            int testNumCores = 0,
            NodeState testState = NodeState.All)
        {
            this.hpcNode = hpcNode;
            this.testCores = (testCores is null) ? this.testCores : testCores;
            this.testName = testName;
            this.testNumCores = testNumCores;
            this.testState = testState;
        }

        /// <summary>
        /// Return the collection of cores this node has, or (if constructed with
        /// a null hpcNode) return the testdata.
        /// </summary>
        /// <returns>A collection of IHipercowSchedulerCore objects.</returns>
        public ISchedulerCollection GetCores()
        {
            if (this.hpcNode is null)
            {
                return this.testCores;
            }

            SchedulerCollection<HipercowSchedulerCore> res = new SchedulerCollection<HipercowSchedulerCore>();
            ISchedulerCollection hpcRes = this.hpcNode.GetCores();
            foreach (ISchedulerCore isc in hpcRes)
            {
                res.Add(new HipercowSchedulerCore(isc));
            }

            return res;
        }

        /// <summary>
        /// Return the name of this node.
        /// </summary>
        /// <returns>the name of the node or (if constructed with
        /// a null hpcNode) return the test name.</returns>
        public string GetName()
        {
            return (this.hpcNode is null) ? this.testName : this.hpcNode.Name;
        }

        /// <summary>
        /// Number of cores this node has available for HPC.
        /// </summary>
        /// <returns>the number of cores (or if constructed with
        /// a null hpcNode) return the test number of cores.</returns>
        public int GetNumCores()
        {
            return (this.hpcNode is null) ? this.testNumCores : this.hpcNode.NumberOfCores;
        }

        /// <summary>
        /// Current state of this node (Offline, Online, etc).
        /// </summary>
        /// <returns>The NodeState (enum) - or if constructed with
        /// a null hpcNode, return the test state.</returns>
        public NodeState GetState()
        {
            return (this.hpcNode is null) ? this.testState : this.hpcNode.State;
        }

        /// <summary>
        /// Convert NodeState to a string.
        /// </summary>
        /// <param name="nodeState">HPC Nodestate object.</param>
        /// <returns>String value of node state.</returns>
        public string GetStateName(NodeState nodeState)
        {
            return (nodeState == NodeState.Online) ? "Online" : "Offline";
        }
    }
}

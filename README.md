# hipercow-api

<!-- badges: start -->
[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)
[![codecov.io](https://codecov.io/gh/mrc-ide/hipercow-api/graph/badge.svg?token=1Zeyvs0lfh)](https://codecov.io/gh/mrc-ide/hipercow-api)
<!-- badges: end -->

This is the .NET Core Web api to talk to our MS HPC Cluster.

# Endpoints

* `/api/v1/Clusters` - List of cluster headnodes we support.
* `/api/v1/Clusters/{cluster}` - Cluster name, maxRam, maxCores, nodes list, queues list and defaultQueue.
* `/api/v1/ClusterLoad/{cluster}` - Cluster name, list of nodeLoads - each of which is name, coresInUse, nodeCores and state.


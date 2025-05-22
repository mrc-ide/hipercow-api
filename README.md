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
* `/api/v1/JobList` - POST cluster, user, state and maxRows to query jobs the headnode knows about with those filters.
* `/metrics`- Endpoint for Prometheus.
* `/api/v1/auth/login` - Authenticate - form entry for user and password, providing a JWT.
* `/api/v1/auth/logout` - Logout removing session.
* `/api/v1/ClusterAccess` - Having authenticated, returns clusters the user has access to.


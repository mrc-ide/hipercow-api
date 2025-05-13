// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using System.Security.Claims;
    using HipercowApi.Controllers;
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Test the /clusteraccess endpoint.
    /// </summary>
    public class ClusterAccessControllerUnitTest
    {
        /// <summary>
        /// Test the /clusteraccess
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterAccess_Works()
        {
            JwtSupport jwtSupport = new JwtSupport();
            string authHeader = "Bearer " +
                jwtSupport.GenerateEncryptedToken("abc", "def", true, false);

            ClusterAccessController cac = new ClusterAccessController(jwtSupport);
            var res = cac.GetMyClusters(authHeader);
            Assert.Equivalent(cac.Ok("wpia-hn"), res);
        }

        /// <summary>
        /// Test the /clusteraccess on non-access user
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterAccessDenied_Works()
        {
            JwtSupport jwtSupport = new JwtSupport();
            string authHeader = "Bearer " +
                jwtSupport.GenerateEncryptedToken("abc", "def", false, false);

            ClusterAccessController cac = new ClusterAccessController(jwtSupport);
            var res = cac.GetMyClusters(authHeader);
            Assert.Equivalent(cac.Unauthorized("Domain authentication ok, but no access to any clusters."), res);
        }
    }
}

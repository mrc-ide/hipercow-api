// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using System.Security.Claims;
    using HipercowApi.Controllers;
    using HipercowApi.Models;
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
            UserSessionManager usm = new(new MemoryCache(new MemoryCacheOptions()));
            ClusterAccessController cac = new ClusterAccessController(usm);
            var user = new ClaimsPrincipal(
               new ClaimsIdentity(
                   new[]
                   {
                        new Claim(ClaimTypes.Name, "abc"),
                        new Claim(ClaimTypes.NameIdentifier, "abc"),
                   },
                   "mock"));

            cac.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                },
            };

            var session = new UserSession
            {
                Username = "abc",
                Password = "def",
                Wpia_hn_access = true,
                Wpia_hn_admin = false,
            };
            var sessionId = usm.StoreSession(session);
            var res = cac.GetMyClusters(sessionId);
            Assert.Equivalent(cac.Ok("wpia-hn"), res);
            usm.RemoveSession(sessionId);

            session = new UserSession
            {
                Username = "abc",
                Password = "def",
                Wpia_hn_access = false,
                Wpia_hn_admin = false,
            };
            sessionId = usm.StoreSession(session);
            res = cac.GetMyClusters(sessionId);
            Assert.Equivalent(cac.Unauthorized("Domain authentication ok, but no access to any clusters."), res);

            usm.RemoveSession(sessionId);
            res = cac.GetMyClusters(sessionId);
            Assert.Equivalent(cac.Unauthorized("Session expired or invalid."), res);
        }
    }
}

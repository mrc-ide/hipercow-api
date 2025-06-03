// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using Hipercow_api.Tools.Exceptions;
    using HipercowApi.Controllers;
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

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
            JwtSupport jwtSupport = new();
            var token = jwtSupport.DecryptToken(
                jwtSupport.GenerateEncryptedToken("abc", "def", true, false));

            Assert.NotNull(token);
            Assert.IsType<Dictionary<string, object>>(token);
            Assert.True(token.ContainsKey("wpia_hn_access"));

            ClusterAccessController cac = new(jwtSupport);
            var httpContext = new DefaultHttpContext();
            httpContext.Items["JwtData"] = token;
            cac.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
            var res = cac.GetMyClusters();
            Assert.Equivalent(cac.Ok("wpia-hn"), res);
        }

        /// <summary>
        /// Test the /clusteraccess on non-access user
        /// by providing fake data.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Fact]
        public void GetClusterAccessDenied_Works()
        {
            JwtSupport jwtSupport = new();
            var token = jwtSupport.DecryptToken(
                jwtSupport.GenerateEncryptedToken("abc", "def", false, false));

            ClusterAccessController cac = new(jwtSupport);
            var httpContext = new DefaultHttpContext();
            httpContext.Items["JwtData"] = token;
            cac.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };

            var ex = Assert.Throws<LdapNoClusterPermissions>(() =>
            {
                cac.GetMyClusters();
            });

            Assert.Equal("LdapNoClusterPermissions", ex.GetType().Name);
        }
    }
}

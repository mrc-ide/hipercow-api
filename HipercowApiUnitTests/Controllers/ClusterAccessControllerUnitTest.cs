// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using HipercowApi.Controllers;
    using HipercowApi.Tools;

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
            string authHeader = "Bearer " +
                jwtSupport.GenerateEncryptedToken("abc", "def", true, false);

            ClusterAccessController cac = new(jwtSupport);
            var res = cac.GetMyClusters(authHeader);
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
            string authHeader = "Bearer " +
                jwtSupport.GenerateEncryptedToken("abc", "def", false, false);

            ClusterAccessController cac = new(jwtSupport);
            try
            {
                cac.GetMyClusters(authHeader);
            }
            catch (Exception ex)
            {
                Assert.Equal("LdapNoClusterPermissions", ex.GetType().Name);
            }
        }
    }
}

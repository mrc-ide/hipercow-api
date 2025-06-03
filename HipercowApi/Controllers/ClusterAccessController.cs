// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Controllers
{
    using Hipercow_api.Tools.Exceptions;
    using HipercowApi.Attributes;
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The /clusteraccess endpoint requires an authentication
    /// token, and provides the list of clusters that a certain
    /// user has access to.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ClusterAccessController"/> class.
    /// </remarks>
    /// <param name="jwtSupport">Our class for encrypting JWTs.</param>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClusterAccessController(JwtSupport jwtSupport) : ControllerBase
    {
        private readonly JwtSupport _jwtSupport = jwtSupport;

        /// <summary>
        /// Authenticated endpoint to return the list of clusters available to a user.
        /// </summary>
        /// <returns>
        /// A list of cluster names.
        /// </returns>
        [HttpGet]
        [RequireJwt]
        public IActionResult GetMyClusters()
        {
            var jwtData = HttpContext.Items["JwtData"] as Dictionary<string, object>;
            var jwtUsername = jwtData!["sub"];
            var wpiahn_access = jwtData["wpia_hn_access"].Equals(true);

            if (wpiahn_access)
            {
                return Ok("wpia-hn");
            }
            else
            {
                throw new LdapNoClusterPermissions((string)jwtUsername);
            }
        }
    }
}

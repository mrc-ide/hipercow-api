// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Controllers
{
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Jose;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The /clusteraccess endpoint requires an authentication
    /// token, and provides the list of clusters that a certain
    /// user has access to.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ClusterAccessController"/> class.
    /// </remarks>
    /// <param name="keys">The JWT Keys.</param>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClusterAccessController(JwtSupport keys) : ControllerBase
    {
        private JwtSupport keys = keys;

        /// <summary>
        /// Authenticated endpoint to return the list of clusters available to a user.
        /// </summary>
        /// <param name="authHeader">Header information to get the encrypted JWT from.</param>
        /// <returns>
        /// A list of cluster names.
        /// </returns>
        [HttpGet]
        public IActionResult GetMyClusters([FromHeader(Name = "Authorization")] string authHeader)
        {
            var token = authHeader.Replace("Bearer ", string.Empty);
            var dict = this.keys.DecryptToken(token);

            var jwtUsername = dict["sub"];
            var wpiahn_access = dict["wpia_hn_access"].Equals(true);

            if (wpiahn_access)
            {
                return this.Ok("wpia-hn");
            }
            else
            {
                return this.Unauthorized("Domain authentication ok, but no access to any clusters.");
            }
        }
    }
}

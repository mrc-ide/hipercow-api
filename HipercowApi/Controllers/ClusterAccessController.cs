// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Controllers
{
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The /clusteraccess endpoint requires an authentication
    /// token, and provides the list of clusters that a certain
    /// user has access to.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClusterAccessController : ControllerBase
    {
        private readonly IClusterHandleCache clusterHandleCache;
        private readonly UserSessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ClusterAccessController"/> class.
        /// </summary>
        /// <param name="clusterHandleCache">The cluster handle cache so we can look up
        /// the connected scheduler object for the requested cluster.
        /// </param>
        /// <param name="sessionManager">The session manager so we can look up the
        /// login details of previous sessions in the memory cache.
        /// </param>
        public ClusterAccessController(
            IClusterHandleCache clusterHandleCache,
            UserSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
            this.clusterHandleCache = clusterHandleCache;
        }

        /// <summary>
        /// Authenticated endpoint to return the list of clusters available to a user.
        /// </summary>
        /// <param name="sessionId">Session Id for retrieving login details.</param>
        /// <returns>
        /// A list of cluster names.
        /// </returns>
        [Authorize]
        [HttpGet]
        public IActionResult SecureAction([FromHeader(Name = "X-Session-Id")] string sessionId)
        {
            var jwtUsername = this.User.Identity!.Name;
            if (string.IsNullOrEmpty(jwtUsername))
            {
                return this.Unauthorized("Missing user identity from token");
            }

            var session = this.sessionManager.RetrieveSession(sessionId);
            if (session == null)
            {
                return this.Unauthorized("Session expired or invalid.");
            }

            if (!string.Equals(session.Username, jwtUsername, StringComparison.OrdinalIgnoreCase))
            {
                return this.Forbid("Session ID does not match the logged-in user.");
            }

            // Use session info:
            if (session.Wpia_hn_access)
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

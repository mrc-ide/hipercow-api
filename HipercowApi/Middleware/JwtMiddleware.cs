// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Middleware
{
    using HipercowApi.Attributes;
    using HipercowApi.Tools;

    /// <summary>
    /// Class for processing JWT token with middleware.
    /// </summary>
    /// <param name="next">Next delegate.</param>
    /// <param name="jwtSupport">Helper class with JWT tools.</param>
    public class JwtMiddleware(RequestDelegate next, JwtSupport jwtSupport)
    {
        private readonly RequestDelegate _next = next;
        private readonly JwtSupport _jwtSupport = jwtSupport;

        /// <summary>
        /// JWT handler.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>Task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint()!;
            var requiresJwt = endpoint.Metadata.GetMetadata<RequireJwtAttribute>() != null;

            if (requiresJwt)
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing or invalid Authorization header.");
                    return;
                }

                try
                {
                    var token = authHeader.Replace("Bearer ", string.Empty);
                    var dict = _jwtSupport.DecryptToken(token);
                    context.Items["JwtData"] = dict;
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid JWT token.");
                    return;
                }
            }

            await _next(context);
        }
    }
}

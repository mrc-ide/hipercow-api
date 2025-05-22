// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowAPI.Tools
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Text.Json;
    using Hipercow_api.Tools.Exceptions;

    /// <summary>
    /// Class for exception middleware.
    /// </summary>
    /// <param name="next">Next delegate.</param>
    /// <param name="logger">Logger.</param>
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        /// <summary>
        /// Error invocation handler.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>Task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse();

            switch (exception)
            {
                case LdapAuthFailure:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Authentication failed";
                    response.Details = exception.Message;
                    break;

                case LdapNoClusterPermissions:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Message = "Domain authentication succeeded, " +
                                       "but no permission to access the cluster.";
                    response.Details = exception.Message;
                    break;

                case LdapEmptyUsernamePassword:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Username and password both need providing " +
                                       "for authentication";
                    response.Details = exception.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An internal server error occurred";
                    response.Details = "Please contact support if the problem persists";
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowAPI.Tools
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Text.Json;
    using Hipercow_api.Tools.Exceptions;
    using HipercowApi.Types;

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
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

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
                var errorId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "Unhandled exception. Error ID: {ErrorId}", errorId);
                await HandleExceptionAsync(context, ex, errorId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string errorId)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse();

            switch (exception)
            {
                case LdapAuthFailure:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = exception.Message;
                    break;

                case LdapNoClusterPermissions:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Message = exception.Message;
                    break;

                case LdapEmptyUsernamePassword:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An internal server error occurred.";
                    response.Details = "Error Id: " + errorId;
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response, _jsonSerializerOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Middleware
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Hipercow_api.Tools.Exceptions;
    using HipercowApi.Middleware;
    using HipercowApi.Types;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;

    /// <summary>
    /// Tests on the ExceptionHandlingMiddleware class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlingMiddlewareUnitTests
    {
        /// <summary>
        /// No exception passes through.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassThru_Works()
        {
            var nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var context = new DefaultHttpContext();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            var ehMiddleware = new ExceptionHandlingMiddleware(next, mockLogger.Object);
            await ehMiddleware.InvokeAsync(context);
            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        }

        /// <summary>
        /// Test known exceptions.
        /// </summary>
        /// <param name="exceptionType">Type of exception for each test.</param>
        /// <param name="expectedStatus">Status code expected for the exception.</param>
        /// <param name="message">Expected error message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(typeof(LdapAuthFailure), 401, "Couldn't authenticate user X")]
        [InlineData(typeof(LdapNoClusterPermissions), 403, "User X does not have permission to use any clusters.")]
        [InlineData(typeof(LdapEmptyUsernamePassword), 400, "User and password for login cannot be empty.")]
        public async Task KnownExceptions_Works(Type exceptionType, int expectedStatus, string message)
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            Exception exception = (exceptionType == typeof(LdapEmptyUsernamePassword)) ?
                (Exception)Activator.CreateInstance(exceptionType)! :
                (Exception)Activator.CreateInstance(exceptionType, "X")!;

            RequestDelegate next = (ctx) => throw exception;
            var ehMiddleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

            // Act
            await ehMiddleware.InvokeAsync(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiErrorResponse>(
                responseText,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                })!;

            // Assert
            Assert.Equal(expectedStatus, context.Response.StatusCode);
            Assert.Equal(expectedStatus, response.StatusCode);
            Assert.Equal(message, response.Message);
        }

        /// <summary>
        /// Test unhandled exception.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task UnknownException_Works()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            RequestDelegate next = (ctx) => throw new InvalidOperationException("Something unexpected");
            var ehMiddleware = new ExceptionHandlingMiddleware(next, mockLogger.Object);

            await ehMiddleware.InvokeAsync(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiErrorResponse>(
                responseText,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                })!;

            Assert.Equal(500, context.Response.StatusCode);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal("An internal server error occurred.", response.Message);
            Assert.StartsWith("Error Id:", response.Details);
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Unhandled exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        }
    }
}

// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Middleware
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using HipercowApi.Attributes;
    using HipercowApi.Middleware;
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Http;
    using Moq;

    /// <summary>
    /// Tests on the JwtMiddleware class, which
    /// helps with endpoints that need a JWT decrypted.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class JwtMiddlewareUnitTests
    {
        /// <summary>
        /// Check quick escape if JWT not needded.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task NonJwt_Works()
        {
            var mockJwtSupport = new Mock<JwtSupport>();
            var nextCalled = false;
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var jwtMiddleware = new JwtMiddleware(next, mockJwtSupport.Object);
            var context = new DefaultHttpContext();
            context.SetEndpoint(new Endpoint(
                (ctx) => Task.CompletedTask,
                new EndpointMetadataCollection(),
                "Test"));

            await jwtMiddleware.InvokeAsync(context);
            Assert.True(nextCalled);
        }

        /// <summary>
        /// Check we get a good error if no authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task JwtBadHeader_Works()
        {
            var mockJwtSupport = new Mock<JwtSupport>();
            var response = new MemoryStream();
            var context = new DefaultHttpContext
            {
                Response = { Body = response },
            };
            context.SetEndpoint(new Endpoint(
                (ctx) => Task.CompletedTask,
                new EndpointMetadataCollection(new RequireJwtAttribute()),
                "Test"));

            var jwtMiddleware = new JwtMiddleware(
                (ctx) => Task.CompletedTask, mockJwtSupport.Object);

            await jwtMiddleware.InvokeAsync(context);
            response.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(response);
            var responseText = await reader.ReadToEndAsync();

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal("Missing or invalid Authorization header.", responseText);
        }

        /// <summary>
        /// Check we get a good error if the JWT is garbage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task JwtBadJWT_Works()
        {
            var mockJwtSupport = new Mock<JwtSupport>();
            mockJwtSupport.Setup(j => j.DecryptToken("wonkytoken")).Throws(new Exception("Decryption failed"));

            var nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = "Bearer wonkytoken";
            context.SetEndpoint(new Endpoint(
                (ctx) => Task.CompletedTask,
                new EndpointMetadataCollection(
                    new RequireJwtAttribute()),
                "Test"));

            var jwtMiddleware = new JwtMiddleware(next, mockJwtSupport.Object);
            var response = new MemoryStream();
            context.Response.Body = response;
            await jwtMiddleware.InvokeAsync(context);

            response.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(response);
            var responseText = await reader.ReadToEndAsync();

            Assert.False(nextCalled);
            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
            Assert.Equal("Invalid JWT token.", responseText);
        }

        /// <summary>
        /// Check everything works if header and JWT are good.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task Jwt_Works()
        {
            var mockJwtSupport = new Mock<JwtSupport>();
            var tokenPayload = new Dictionary<string, object> { { "drink", "tea" } };
            mockJwtSupport.Setup(j => j.DecryptToken("goodtoken")).Returns(tokenPayload);

            var nextCalled = false;
            RequestDelegate next = (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = "Bearer goodtoken";
            context.SetEndpoint(new Endpoint(
                (ctx) => Task.CompletedTask,
                new EndpointMetadataCollection(
                    new RequireJwtAttribute()),
                "Test"));

            var jwtMiddleware = new JwtMiddleware(next, mockJwtSupport.Object);
            await jwtMiddleware.InvokeAsync(context);

            Assert.True(nextCalled);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
            Assert.True(context.Items.ContainsKey("JwtData"));
            var dict = (Dictionary<string, object>)context.Items["JwtData"]!;
            Assert.Equal("tea", dict["drink"]);
        }
    }
}

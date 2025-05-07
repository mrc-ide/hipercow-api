// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.DirectoryServices.Protocols;
    using System.Security.Claims;
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;

    /// <summary>
    /// Test the /auth endpoints.
    /// </summary>
    public class AuthControllerUnitTest
    {
        /// <summary>
        /// Test for empty login.
        /// </summary>
        [Fact]
        public void AuthInvalidLogin_Works()
        {
            JwtSettings jwtSettings = new() { SecretKey = "key", Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 1 };
            AuthController ac = new AuthController(
                new JwtTokenGenerator(jwtSettings),
                new UserSessionManager(new MemoryCache(new MemoryCacheOptions())),
                new LdapManager());

            LoginRequest request = new LoginRequest { Username = string.Empty, Password = string.Empty };
            IActionResult res = ac.Login(request);
            Assert.Equivalent(res, ac.BadRequest("Username or password cannot be empty."));

            request = new LoginRequest { Username = "bob", Password = string.Empty };
            res = ac.Login(request);
            Assert.Equivalent(res, ac.BadRequest("Username or password cannot be empty."));
        }

        /// <summary>
        /// Test for invalid credentials.
        /// </summary>
        [Fact]
        public void AuthLoginFail_Works()
        {
            JwtSettings jwtSettings = new() { SecretKey = "key", Issuer = "issuer", Audience = "audience", ExpiresInMinutes = 1 };
            var mockLM = new Mock<ILdapManager>();
            AuthController ac = new AuthController(
                new JwtTokenGenerator(jwtSettings),
                new UserSessionManager(new MemoryCache(new MemoryCacheOptions())),
                mockLM.Object);

            LoginRequest request = new LoginRequest { Username = "abc", Password = "def" };
            LdapConnectionWrapper failed = new LdapConnectionWrapper
            {
                Connection = null,
                Result = ac.BadRequest("Failed"),
            };
            mockLM.Setup(x => x.GetDideLdapConnection(ac, request)).Returns(failed);
            IActionResult res = ac.Login(request);
            Assert.Equivalent(res, ac.BadRequest("Failed"));
        }

        /// <summary>
        /// Test for invalid credentials.
        /// </summary>
        [Fact]
        [ExcludeFromCodeCoverage]
        public void AuthLogin_Works()
        {
            UserSessionManager usm = new(new MemoryCache(new MemoryCacheOptions()));
            JwtSettings jwtSettings = new()
            {
                SecretKey = "1234567890123456789012345678901234",
                Issuer = "issuer",
                Audience = "audience",
                ExpiresInMinutes = 1,
            };
            var mockLM = new Mock<ILdapManager>();
            AuthController ac = new(
                new JwtTokenGenerator(jwtSettings),
                usm,
                mockLM.Object);

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, "abc"),
                        new Claim(ClaimTypes.NameIdentifier, "abc"),
                    },
                    "mock"));

            ac.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                },
            };

            LdapDirectoryIdentifier fakeLdapDirId = new("www.server.com", 389);
            LdapConnection fakeLdap = new(fakeLdapDirId);

            LoginRequest request = new() { Username = "abc", Password = "def" };
            LdapConnectionWrapper success = new()
            {
                Connection = fakeLdap,
                Result = ac.Ok(),
            };

            mockLM.Setup(x => x.GetDideLdapConnection(ac, request)).Returns(success);
            mockLM.Setup(x => x.GetDomainGroups("abc", success.Connection)).Returns(["WPIA-HN.HPC Users - All Nodes"]);
            IActionResult res = ac.Login(request);
            var okResult = Assert.IsType<OkObjectResult>(res);
            dynamic? value = okResult.Value;
            string token = value!.token;
            string sessionId = value!.sessionId;

            Assert.NotNull(token);
            Assert.NotNull(sessionId);

            UserSession? session = usm.RetrieveSession(sessionId);
            Assert.NotNull(session);
            Assert.Equal("abc", session.Username);
            Assert.Equal("def", session.Password);
            Assert.False(session.Wpia_hn_admin);
            Assert.True(session.Wpia_hn_access);

            res = ac.Logout(sessionId);
            session = usm.RetrieveSession(sessionId);
            Assert.Null(session);

            res = ac.Logout(sessionId);
            Assert.Equivalent(ac.Unauthorized("Session expired or invalid."), res);
        }
    }
}

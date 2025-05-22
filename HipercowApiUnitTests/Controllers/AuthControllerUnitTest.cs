// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.DirectoryServices.Protocols;
    using System.Net;
    using System.Security.Claims;
    using Hipercow_api.Tools.Exceptions;
    using HipercowApi.Models;
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
        [ExcludeFromCodeCoverage]
        public void AuthInvalidLogin_Works()
        {
            JwtSupport jwtSupport = new();
            AuthController ac = new(
                jwtSupport,
                new LdapManager());

            LoginRequest request = new() { Username = string.Empty, Password = string.Empty };
            Assert.Throws<LdapEmptyUsernamePassword>(() => ac.Login(request));

            request = new() { Username = "bob", Password = string.Empty };
            Assert.Throws<LdapEmptyUsernamePassword>(() => ac.Login(request));
        }

        /// <summary>
        /// Test for invalid credentials.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Fact]
        public void AuthLoginFail_Works()
        {
            JwtSupport jwtSupport = new();
            var mockLM = new Mock<ILdapManager>();
            NetworkCredential creds = new NetworkCredential("abc", "def", "domain.com");
            mockLM.Setup(x => x.GetLdapCredentials(It.IsAny<LoginRequest>())).Returns(creds);
            mockLM.Setup(x => x.GetLdapConnection(It.IsAny<NetworkCredential>())).Verifiable();
            mockLM.Setup(x => x.DoBind(
                It.IsAny<LdapConnection>(),
                It.IsAny<NetworkCredential>()))
                  .Throws(new LdapAuthFailure("abc"));

            AuthController ac = new(
                jwtSupport,
                mockLM.Object);

            LoginRequest request = new() { Username = "abc", Password = "def" };
            Assert.Throws<LdapAuthFailure>(() => ac.Login(request));
        }

        /// <summary>
        /// Test for invalid credentials.
        /// </summary>
        [Fact]
        [ExcludeFromCodeCoverage]
        public void AuthLogin_Works()
        {
            JwtSupport jwtSupport = new JwtSupport();
            var mockLM = new Mock<ILdapManager>();
            AuthController ac = new(
                jwtSupport,
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

            mockLM.Setup(x => x.GetLdapConnection(It.IsAny<NetworkCredential>()))
                .Returns(fakeLdap);
            mockLM.Setup(x => x.DoBind(
                It.IsAny<LdapConnection>(),
                It.IsAny<NetworkCredential>())).Verifiable();
            mockLM.Setup(x => x.GetDomainGroups("abc", fakeLdap)).Returns(["WPIA-HN.HPC Users - All Nodes"]);
            IActionResult res = ac.Login(request);
            var okResult = Assert.IsType<OkObjectResult>(res);
            dynamic? token = okResult.Value;
            Assert.NotNull(token);
        }
    }
}

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using Xunit;

namespace PersonalFinanceTracker.Tests.UnitTests.ControllerTests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk_WhenServiceSucceeds()
        {
            var authMock = new Mock<IAuthService>();
            var refreshMock = new Mock<IRefreshTokenService>();
            authMock.Setup(a => a.RegisterAsync(It.IsAny<RegisterRequest>()))
                    .ReturnsAsync("registered");

            var controller = new AuthController(authMock.Object, refreshMock.Object);

            var req = new RegisterRequest("user1", "user1@example.com", "Password1A");
            var result = await controller.Register(req);

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().Be("registered");
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsValid()
        {
            var authMock = new Mock<IAuthService>();
            var refreshMock = new Mock<IRefreshTokenService>();

            var response = new AuthResponse("access", "refresh", "user1");
            authMock.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>()))
                    .ReturnsAsync(response);

            var controller = new AuthController(authMock.Object, refreshMock.Object);
            var req = new LoginRequest("user1@example.com", "Password1A");

            var result = await controller.Login(req);

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenServiceThrows()
        {
            var authMock = new Mock<IAuthService>();
            var refreshMock = new Mock<IRefreshTokenService>();

            authMock.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>()))
                    .ThrowsAsync(new Exception("invalid"));

            var controller = new AuthController(authMock.Object, refreshMock.Object);
            var req = new LoginRequest("user1@example.com", "bad");

            var result = await controller.Login(req);

            result.Should().BeOfType<UnauthorizedObjectResult>();
            var una = result as UnauthorizedObjectResult;
            una!.Value.Should().Be("invalid");
        }

        [Fact]
        public async Task Refresh_ReturnsOk_WhenRefreshSucceeds()
        {
            var authMock = new Mock<IAuthService>();
            var refreshMock = new Mock<IRefreshTokenService>();

            var response = new AuthResponse("access", "refresh", "user1");
            refreshMock.Setup(r => r.RefreshTokenAsync(It.IsAny<string>()))
                       .ReturnsAsync(response);

            var controller = new AuthController(authMock.Object, refreshMock.Object);
            var req = new RefreshRequest("some-refresh-token");

            var result = await controller.Refresh(req);

            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeSameAs(response);
        }

        [Fact]
        public async Task Refresh_ReturnsUnauthorized_WhenServiceThrows()
        {
            var authMock = new Mock<IAuthService>();
            var refreshMock = new Mock<IRefreshTokenService>();

            refreshMock.Setup(r => r.RefreshTokenAsync(It.IsAny<string>()))
                       .ThrowsAsync(new Exception("bad refresh"));

            var controller = new AuthController(authMock.Object, refreshMock.Object);
            var req = new RefreshRequest("invalid-token");

            var result = await controller.Refresh(req);

            result.Should().BeOfType<UnauthorizedObjectResult>();
            var una = result as UnauthorizedObjectResult;
            una!.Value.Should().Be("bad refresh");
        }
    }
}

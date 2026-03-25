using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Application.Settings;
using PersonalFinanceTracker.Domain.Entities;
using PersonalFinanceTracker.Infrastructure.Services;
using Xunit;

namespace PersonalFinanceTracker.Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private IUnitOfWork CreateUnitOfWorkMock(Action<Mock<IUnitOfWork>>? setup = null)
        {
            var uowMock = new Mock<IUnitOfWork>();
            var userRepo = new Mock<IUserRepository>();
            var refreshRepo = new Mock<IRefreshTokenRepository>();

            uowMock.SetupGet(u => u.Users).Returns(userRepo.Object);
            uowMock.SetupGet(u => u.RefreshTokens).Returns(refreshRepo.Object);

            setup?.Invoke(uowMock);
            return uowMock.Object;
        }

        [Fact]
        public async Task RegisterAsync_Succeeds_WhenEmailNotExists()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(u => u.Users).Returns(userRepoMock.Object);
            uowMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var opts = Options.Create(new JwtOptions { Key = "test_key_which_is_long_enough_1234567890", Issuer = "test", Audience = "test", AccessTokenExpirationMinutes = 60, RefreshTokenExpirationDays = 7 });
            var svc = new AuthService(opts, uowMock.Object);

            var req = new RegisterRequest("u1", "u1@example.com", "Password1A");
            var res = await svc.RegisterAsync(req);

            res.Should().Be("Đăng ký tài khoản thành công.");
            userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == req.Email && u.Username == req.Username)), Times.Once);
            uowMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Throws_WhenEmailExists()
        {
            var existing = new User { Id = Guid.NewGuid(), Email = "a@a.com" };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(existing);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(u => u.Users).Returns(userRepoMock.Object);

            var opts = Options.Create(new JwtOptions { Key = "key", Issuer = "i", Audience = "a", AccessTokenExpirationMinutes = 60, RefreshTokenExpirationDays = 7 });
            var svc = new AuthService(opts, uowMock.Object);

            await Assert.ThrowsAsync<Exception>(() => svc.RegisterAsync(new RegisterRequest("u","a@a.com","pass")));
        }

        [Fact]
        public async Task LoginAsync_Succeeds_ReturnsAuthResponse()
        {
            var password = "Password1A";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User { Id = Guid.NewGuid(), Email = "u@example.com", Username = "u1", PasswordHash = hash };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var refreshRepoMock = new Mock<IRefreshTokenRepository>();
            refreshRepoMock.Setup(r => r.AddAsync(It.IsAny<UserRefreshToken>())).Returns(Task.CompletedTask);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(u => u.Users).Returns(userRepoMock.Object);
            uowMock.SetupGet(u => u.RefreshTokens).Returns(refreshRepoMock.Object);
            uowMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            var opts = Options.Create(new JwtOptions { Key = "another_long_test_key_1234567890", Issuer = "i", Audience = "a", AccessTokenExpirationMinutes = 60, RefreshTokenExpirationDays = 7 });
            var svc = new AuthService(opts, uowMock.Object);

            var response = await svc.LoginAsync(new LoginRequest(user.Email, password));

            response.Should().NotBeNull();
            response.Username.Should().Be(user.Username);
            response.AccessToken.Should().NotBeNullOrWhiteSpace();
            response.RefreshToken.Should().NotBeNullOrWhiteSpace();

            refreshRepoMock.Verify(r => r.AddAsync(It.IsAny<UserRefreshToken>()), Times.Once);
            uowMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenInvalidCredentials()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(u => u.Users).Returns(userRepoMock.Object);

            var opts = Options.Create(new JwtOptions { Key = "k", Issuer = "i", Audience = "a", AccessTokenExpirationMinutes = 60, RefreshTokenExpirationDays = 7 });
            var svc = new AuthService(opts, uowMock.Object);

            await Assert.ThrowsAsync<Exception>(() => svc.LoginAsync(new LoginRequest("no@no.com","x")));
        }
    }
}

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
    public class RefreshTokenServiceTests
    {
        [Fact]
        public async Task RefreshTokenAsync_Throws_WhenTokenNotFoundOrExpired()
        {
            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.RefreshTokens).Returns(Mock.Of<IRefreshTokenRepository>(r => r.GetByTokenHashAsync(It.IsAny<string>()) == Task.FromResult<UserRefreshToken?>(null)));
            uow.SetupGet(x => x.Users).Returns(Mock.Of<IUserRepository>(r => r.GetByIdAsync(It.IsAny<Guid>()) == Task.FromResult<User?>(null)));

            var opts = Options.Create(new JwtOptions { Key = "k", Issuer = "i", Audience = "a", AccessTokenExpirationMinutes = 60, RefreshTokenExpirationDays = 7 });
            var svc = new RefreshTokenService(opts, uow.Object);

            await Assert.ThrowsAsync<Exception>(() => svc.RefreshTokenAsync("badtoken"));
        }

        [Fact]
        public async Task RefreshTokenAsync_Succeeds_ReturnsAuthResponse()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "a@a.com", Username = "u1" };
            var existing = new UserRefreshToken { Id = Guid.NewGuid(), UserId = user.Id, TokenHash = "hash", ExpiresAt = DateTime.UtcNow.AddDays(1), RevokedAt = null };

            var refreshRepo = new Mock<IRefreshTokenRepository>();
            refreshRepo.Setup(r => r.GetByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(existing);
            refreshRepo.Setup(r => r.AddAsync(It.IsAny<UserRefreshToken>())).Returns(Task.CompletedTask);
            refreshRepo.Setup(r => r.Update(It.IsAny<UserRefreshToken>()));

            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByIdAsync(existing.UserId)).ReturnsAsync(user);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(x => x.RefreshTokens).Returns(refreshRepo.Object);
            uowMock.SetupGet(x => x.Users).Returns(userRepo.Object);
            uowMock.Setup(x => x.CompleteAsync()).ReturnsAsync(1);

            var opts = Options.Create(new JwtOptions { Key = new string('k', 64), Issuer = "i", Audience = "a", AccessTokenExpirationMinutes = 60, RefreshTokenExpirationDays = 7 });
            var svc = new RefreshTokenService(opts, uowMock.Object);

            var res = await svc.RefreshTokenAsync("somerawtoken");

            res.Should().NotBeNull();
            res.Username.Should().Be(user.Username);
            res.AccessToken.Should().NotBeNullOrWhiteSpace();
            res.RefreshToken.Should().NotBeNullOrWhiteSpace();

            refreshRepo.Verify(r => r.AddAsync(It.IsAny<UserRefreshToken>()), Times.Once);
            refreshRepo.Verify(r => r.Update(It.Is<UserRefreshToken>(t => t.ReplacedByTokenHash != null && t.RevokedAt != null)), Times.Once);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}

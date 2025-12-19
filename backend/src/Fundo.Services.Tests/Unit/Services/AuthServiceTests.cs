using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Application.Services;
using Fundo.Applications.WebApi.Infrastructure.Auth;
using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Services
{
    public class AuthServiceTests
    {
        private static JwtTokenService CreateJwt()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Fundo",
                ["Jwt:Audience"] = "Fundo",
                ["Jwt:Key"] = "SUPER_LONG_SECRET_KEY_CHANGE_ME_32+_CHARS"
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            return new JwtTokenService(config);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailExists_ShouldThrow()
        {
            var users = new Mock<IUserRepository>();
            var uow = new Mock<IUnitOfWork>();
            var jwt = CreateJwt();

            users.Setup(r => r.ExistsByEmailAsync("a@a.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var service = new AuthService(users.Object, uow.Object, jwt);

            Func<Task> act = async () => await service.RegisterAsync(new AuthRequestDto
            {
                Email = "a@a.com",
                PasswordHash = "HASH_FROM_FRONTEND_1234567890"
            });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Email already registered.");
        }

        [Fact]
        public async Task RegisterAsync_WhenValid_ShouldPersistUserAndReturnToken()
        {
            var users = new Mock<IUserRepository>();
            var uow = new Mock<IUnitOfWork>();
            var jwt = CreateJwt();

            users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            AppUser? captured = null;
            users.Setup(r => r.AddAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()))
                 .Callback<AppUser, CancellationToken>((u, _) => captured = u)
                 .Returns(Task.CompletedTask);

            uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

            var service = new AuthService(users.Object, uow.Object, jwt);

            var resp = await service.RegisterAsync(new AuthRequestDto
            {
                Email = "new@new.com",
                PasswordHash = "HASH_FROM_FRONTEND_1234567890"
            });

            captured.Should().NotBeNull();
            captured!.Email.Should().Be("new@new.com");
            captured.PasswordHash.Should().Be("HASH_FROM_FRONTEND_1234567890");

            resp.AccessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ShouldThrow()
        {
            var users = new Mock<IUserRepository>();
            var uow = new Mock<IUnitOfWork>();
            var jwt = CreateJwt();

            users.Setup(r => r.GetByEmailAsync("x@x.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((AppUser?)null);

            var service = new AuthService(users.Object, uow.Object, jwt);

            Func<Task> act = async () => await service.LoginAsync(new AuthRequestDto
            {
                Email = "x@x.com",
                PasswordHash = "HASH"
            });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Invalid credentials.");
        }

        [Fact]
        public async Task LoginAsync_WhenHashMismatch_ShouldThrow()
        {
            var users = new Mock<IUserRepository>();
            var uow = new Mock<IUnitOfWork>();
            var jwt = CreateJwt();

            users.Setup(r => r.GetByEmailAsync("x@x.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new AppUser { Email = "x@x.com", PasswordHash = "HASH_DB" });

            var service = new AuthService(users.Object, uow.Object, jwt);

            Func<Task> act = async () => await service.LoginAsync(new AuthRequestDto
            {
                Email = "x@x.com",
                PasswordHash = "HASH_CLIENT"
            });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Invalid credentials.");
        }

        [Fact]
        public async Task LoginAsync_WhenValid_ShouldReturnToken()
        {
            var users = new Mock<IUserRepository>();
            var uow = new Mock<IUnitOfWork>();
            var jwt = CreateJwt();

            var user = new AppUser { Id = Guid.NewGuid(), Email = "ok@ok.com", PasswordHash = "HASH_OK" };

            users.Setup(r => r.GetByEmailAsync("ok@ok.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

            var service = new AuthService(users.Object, uow.Object, jwt);

            var resp = await service.LoginAsync(new AuthRequestDto
            {
                Email = "ok@ok.com",
                PasswordHash = "HASH_OK"
            });

            resp.AccessToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}

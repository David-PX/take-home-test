using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;
using Fundo.Services.Tests.Integration.Infrastructure;
using Xunit;

namespace Fundo.Services.Tests.Integration.Repositories
{
    public class UserRepositoryTests : IClassFixture<SqliteDatabaseFixture>
    {
        private readonly SqliteDatabaseFixture fixture;

        public UserRepositoryTests(SqliteDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenUserExists_ShouldReturnTrue()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            db.Users.Add(new AppUser { Email = "a@a.com", PasswordHash = "HASH" });
            await db.SaveChangesAsync();

            var repo = new UserRepository(db);

            var exists = await repo.ExistsByEmailAsync("a@a.com");

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var repo = new UserRepository(db);

            var exists = await repo.ExistsByEmailAsync("missing@a.com");

            exists.Should().BeFalse();
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserExists_ShouldReturnUser()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            db.Users.Add(new AppUser { Email = "ok@ok.com", PasswordHash = "HASH_OK" });
            await db.SaveChangesAsync();

            var repo = new UserRepository(db);

            var user = await repo.GetByEmailAsync("ok@ok.com");

            user.Should().NotBeNull();
            user!.Email.Should().Be("ok@ok.com");
        }

        [Fact]
        public async Task GetByEmailAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var repo = new UserRepository(db);

            var user = await repo.GetByEmailAsync("missing@missing.com");

            user.Should().BeNull();
        }
    }
}

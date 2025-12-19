using System.Threading.Tasks;
using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fundo.Services.Tests.Integration.Infrastructure
{
    public sealed class SqliteDatabaseFixture : IAsyncLifetime
    {
        private SqliteConnection connection = default!;

        public DbContextOptions<AppDbContext> Options { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            Options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .EnableSensitiveDataLogging()
                .Options;

            await using var db = new AppDbContext(Options);
            await db.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await connection.DisposeAsync();
        }
    }
}

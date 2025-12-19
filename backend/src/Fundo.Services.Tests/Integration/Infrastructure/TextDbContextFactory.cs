using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Services.Tests.Integration.Infrastructure
{
    public static class TestDbContextFactory
    {
        public static AppDbContext Create(DbContextOptions<AppDbContext> options)
            => new AppDbContext(options);
    }
}

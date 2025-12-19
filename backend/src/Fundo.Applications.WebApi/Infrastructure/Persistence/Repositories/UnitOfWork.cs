using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext db;
        public UnitOfWork(AppDbContext db) => this.db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => db.SaveChangesAsync(ct);
    }
}

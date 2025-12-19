using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext db;
        public UserRepository(AppDbContext db) => this.db = db;

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => db.Users.AnyAsync(x => x.Email == email, ct);

        public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
            => db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);

        public Task AddAsync(AppUser user, CancellationToken ct = default)
            => db.Users.AddAsync(user, ct).AsTask();
    }
}

using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Infrastructure.Persistence;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task AddAsync(AppUser user, CancellationToken ct = default);
    }
}

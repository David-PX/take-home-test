using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}

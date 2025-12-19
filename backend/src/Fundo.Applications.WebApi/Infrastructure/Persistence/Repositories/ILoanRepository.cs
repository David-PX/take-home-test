using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Domain.Entities;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public interface ILoanRepository
    {
        Task AddAsync(Loan loan, CancellationToken ct = default);
        Task<Loan?> GetByIdWithCustomerAsync(Guid id, CancellationToken ct = default);
        Task<List<Loan>> GetAllWithCustomerAsync(CancellationToken ct = default);
    }
}

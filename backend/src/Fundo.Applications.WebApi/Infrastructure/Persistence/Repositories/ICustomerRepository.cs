using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Domain.Entities;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer, CancellationToken ct = default);
        Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}

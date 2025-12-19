using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext db;
        public CustomerRepository(AppDbContext db) => this.db = db;

        public async Task AddAsync(Customer customer, CancellationToken ct = default)
        {
            await db.Customers.AddAsync(customer, ct);
        }

        public Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
        {
            return db.Customers
                     .AsNoTracking()
                     .ToListAsync(ct);
        }

        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => db.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}

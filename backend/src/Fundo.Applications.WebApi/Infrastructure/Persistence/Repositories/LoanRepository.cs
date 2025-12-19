using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext db;
        public LoanRepository(AppDbContext db) => this.db = db;

        public Task AddAsync(Loan loan, CancellationToken ct = default)
            => db.Loans.AddAsync(loan, ct).AsTask();

        public Task<Loan?> GetByIdWithCustomerAsync(Guid id, CancellationToken ct = default)
            => db.Loans.Include(x => x.Customer)
                       .FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<List<Loan>> GetAllWithCustomerAsync(CancellationToken ct = default)
            => db.Loans.Include(x => x.Customer)
                       .OrderByDescending(x => x.CreatedAtUtc)
                       .ToListAsync(ct);
    }
}

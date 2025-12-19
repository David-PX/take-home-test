using System;
using System.Linq;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Domain.Entities;
using Fundo.Applications.WebApi.Domain.Enums;
using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.WebApi.Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            if (!await db.Customers.AnyAsync())
            {
                var maria = new Customer { FullName = "Maria Silva", Email = "maria@example.com" };
                var john = new Customer { FullName = "John Doe", Email = "john@example.com" };

                db.Customers.AddRange(maria, john);

                db.Loans.AddRange(
                    new Loan { Customer = maria, OriginalAmount = 1500m, CurrentBalance = 500m, Status = LoanStatus.Active },
                    new Loan { Customer = john, OriginalAmount = 900m, CurrentBalance = 0m, Status = LoanStatus.Paid }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}

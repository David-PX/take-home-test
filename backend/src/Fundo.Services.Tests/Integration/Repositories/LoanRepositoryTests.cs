using System;
using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Applications.WebApi.Domain.Entities;
using Fundo.Applications.WebApi.Domain.Enums;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;
using Fundo.Services.Tests.Integration.Infrastructure;
using Xunit;

namespace Fundo.Services.Tests.Integration.Repositories
{
    public class LoanRepositoryTests : IClassFixture<SqliteDatabaseFixture>
    {
        private readonly SqliteDatabaseFixture fixture;

        public LoanRepositoryTests(SqliteDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task AddAsync_WhenCalled_ShouldPersistLoan()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var customer = new Customer { FullName = "John Doe", Email = "john@test.com" };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            var repo = new LoanRepository(db);

            var loan = new Loan
            {
                CustomerId = customer.Id,
                OriginalAmount = 1000m,
                CurrentBalance = 1000m,
                Status = LoanStatus.Active
            };

            await repo.AddAsync(loan);
            await db.SaveChangesAsync();

            var persisted = await db.Loans.FindAsync(loan.Id);
            persisted.Should().NotBeNull();
            persisted!.CustomerId.Should().Be(customer.Id);
        }

        [Fact]
        public async Task GetByIdWithCustomerAsync_WhenLoanExists_ShouldReturnLoanIncludingCustomer()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var customer = new Customer { FullName = "Maria Silva", Email = "maria@test.com" };
            var loan = new Loan
            {
                Customer = customer,
                OriginalAmount = 1500m,
                CurrentBalance = 500m,
                Status = LoanStatus.Active
            };

            db.Loans.Add(loan);
            await db.SaveChangesAsync();

            var repo = new LoanRepository(db);

            var result = await repo.GetByIdWithCustomerAsync(loan.Id);

            result.Should().NotBeNull();
            result!.Customer.Should().NotBeNull();
            result.Customer.FullName.Should().Be("Maria Silva");
        }

        [Fact]
        public async Task GetByIdWithCustomerAsync_WhenLoanDoesNotExist_ShouldReturnNull()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var repo = new LoanRepository(db);

            var result = await repo.GetByIdWithCustomerAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}

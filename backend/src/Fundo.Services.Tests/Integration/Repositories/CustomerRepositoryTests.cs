using System;
using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Applications.WebApi.Domain.Entities;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;
using Fundo.Services.Tests.Integration.Infrastructure;
using Xunit;

namespace Fundo.Services.Tests.Integration.Repositories
{
    public class CustomerRepositoryTests : IClassFixture<SqliteDatabaseFixture>
    {
        private readonly SqliteDatabaseFixture fixture;

        public CustomerRepositoryTests(SqliteDatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetByIdAsync_WhenCustomerExists_ShouldReturnCustomer()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var customer = new Customer { FullName = "Maria Silva", Email = "maria@test.com" };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            var repo = new CustomerRepository(db);

            var result = await repo.GetByIdAsync(customer.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(customer.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
        {
            await using var db = TestDbContextFactory.Create(fixture.Options);

            var repo = new CustomerRepository(db);

            var result = await repo.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}

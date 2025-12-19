using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Application.Services;
using Fundo.Applications.WebApi.Domain.Entities;
using Fundo.Applications.WebApi.Domain.Enums;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;
using Fundo.Services.Tests.Unit.Mapping;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit.Services
{
    public class LoanServiceTests
    {
        private readonly IMapper mapper;

        public LoanServiceTests()
        {
            mapper = AutoMapperFixture.CreateMapper();
        }

        [Fact]
        public async Task CreateAsync_WhenCustomerNotFound_ShouldThrow()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Customer?)null);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            Func<Task> act = async () => await service.CreateAsync(new CreateLoanRequestDto
            {
                CustomerId = Guid.NewGuid(),
                Amount = 100m
            });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Customer not found.");
        }

        [Fact]
        public async Task CreateAsync_WhenValid_ShouldPersistLoanAndReturnMappedDetailDto()
        {
            var customer = new Customer { Id = Guid.NewGuid(), FullName = "Maria Silva", Email = "maria@test.com" };

            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            customerRepo.Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(customer);

            Loan? capturedLoan = null;
            loanRepo.Setup(r => r.AddAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()))
                    .Callback<Loan, CancellationToken>((l, _) => capturedLoan = l)
                    .Returns(Task.CompletedTask);

            uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            var result = await service.CreateAsync(new CreateLoanRequestDto
            {
                CustomerId = customer.Id,
                Amount = 1500m
            });

            capturedLoan.Should().NotBeNull();
            capturedLoan!.CustomerId.Should().Be(customer.Id);
            capturedLoan.OriginalAmount.Should().Be(1500m);
            capturedLoan.CurrentBalance.Should().Be(1500m);
            capturedLoan.Status.Should().Be(LoanStatus.Active);

            result.Customer.FullName.Should().Be("Maria Silva");
            result.CurrentBalance.Should().Be(1500m);
            result.Status.Should().Be("active");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            loanRepo.Setup(r => r.GetByIdWithCustomerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Loan?)null);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            var result = await service.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_ShouldReturnMappedDetailDto()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Customer = new Customer { Id = Guid.NewGuid(), FullName = "John Doe", Email = "john@test.com" },
                OriginalAmount = 100m,
                CurrentBalance = 40m,
                Status = LoanStatus.Active
            };

            loanRepo.Setup(r => r.GetByIdWithCustomerAsync(loan.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(loan);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            var result = await service.GetByIdAsync(loan.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(loan.Id);
            result.Customer.FullName.Should().Be("John Doe");
            result.Status.Should().Be("active");
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldReturnMappedList()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            var list = new List<Loan>
            {
                new Loan
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    Customer = new Customer { Id = Guid.NewGuid(), FullName = "C1" },
                    OriginalAmount = 200m,
                    CurrentBalance = 200m,
                    Status = LoanStatus.Active
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    Customer = new Customer { Id = Guid.NewGuid(), FullName = "C2" },
                    OriginalAmount = 300m,
                    CurrentBalance = 100m,
                    Status = LoanStatus.Active
                }
            };

            loanRepo.Setup(r => r.GetAllWithCustomerAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(list);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
            result[0].Customer.FullName.Should().Be("C1");
            result[1].Customer.FullName.Should().Be("C2");
        }

        [Fact]
        public async Task PayAsync_WhenLoanNotFound_ShouldThrow()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            loanRepo.Setup(r => r.GetByIdWithCustomerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Loan?)null);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            Func<Task> act = async () => await service.PayAsync(Guid.NewGuid(), new LoanPaymentRequestDto { Amount = 10m });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Loan not found.");
        }

        [Fact]
        public async Task PayAsync_WhenLoanIsPaid_ShouldThrow()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            var loan = new Loan { Id = Guid.NewGuid(), Status = LoanStatus.Paid, CurrentBalance = 0m };

            loanRepo.Setup(r => r.GetByIdWithCustomerAsync(loan.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(loan);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            Func<Task> act = async () => await service.PayAsync(loan.Id, new LoanPaymentRequestDto { Amount = 1m });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Loan is already paid.");
        }

        [Fact]
        public async Task PayAsync_WhenAmountExceedsBalance_ShouldThrow()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            var loan = new Loan { Id = Guid.NewGuid(), Status = LoanStatus.Active, CurrentBalance = 20m };

            loanRepo.Setup(r => r.GetByIdWithCustomerAsync(loan.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(loan);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            Func<Task> act = async () => await service.PayAsync(loan.Id, new LoanPaymentRequestDto { Amount = 30m });

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Payment amount cannot exceed current balance.");
        }

        [Fact]
        public async Task PayAsync_WhenPaymentCompletesLoan_ShouldSetPaid()
        {
            var customerRepo = new Mock<ICustomerRepository>();
            var loanRepo = new Mock<ILoanRepository>();
            var uow = new Mock<IUnitOfWork>();

            var loan = new Loan { Id = Guid.NewGuid(), Status = LoanStatus.Active, CurrentBalance = 40m };

            loanRepo.Setup(r => r.GetByIdWithCustomerAsync(loan.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(loan);

            uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

            var service = new LoanService(customerRepo.Object, loanRepo.Object, uow.Object, mapper);

            var result = await service.PayAsync(loan.Id, new LoanPaymentRequestDto { Amount = 40m });

            loan.Status.Should().Be(LoanStatus.Paid);
            loan.CurrentBalance.Should().Be(0m);
            result.Status.Should().Be("paid");
        }
    }
}

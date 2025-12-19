using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Domain.Entities;
using Fundo.Applications.WebApi.Domain.Enums;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;

namespace Fundo.Applications.WebApi.Application.Services
{
    public class LoanService
    {
        private readonly ICustomerRepository customers;
        private readonly ILoanRepository loans;
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public LoanService(
            ICustomerRepository customers,
            ILoanRepository loans,
            IUnitOfWork uow,
            IMapper mapper)
        {
            this.customers = customers;
            this.loans = loans;
            this.uow = uow;
            this.mapper = mapper;
        }

        public async Task<LoanDetailDto> CreateAsync(CreateLoanRequestDto request, CancellationToken ct = default)
        {
            var customer = await customers.GetByIdAsync(request.CustomerId, ct);
            if (customer == null) throw new InvalidOperationException("Customer not found.");

            // AutoMapper mapea el request al Loan base (OriginalAmount desde Amount, CustomerId, etc.)
            var loan = mapper.Map<Loan>(request);

            // Reglas de negocio aquí (no en el mapeo)
            loan.CurrentBalance = request.Amount;
            loan.Status = LoanStatus.Active;

            await loans.AddAsync(loan, ct);
            await uow.SaveChangesAsync(ct);

            // Nota: loan.Customer no está cargado aquí (no hicimos include),
            // pero el DTO de detalle requiere Customer. Lo resolvemos con el customer ya cargado:
            loan.Customer = customer;

            return mapper.Map<LoanDetailDto>(loan);
        }

        public async Task<LoanDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var loan = await loans.GetByIdWithCustomerAsync(id, ct);
            if (loan == null) return null;

            return mapper.Map<LoanDetailDto>(loan);
        }

        public async Task<LoanListItemDto[]> GetAllAsync(CancellationToken ct = default)
        {
            var list = await loans.GetAllWithCustomerAsync(ct);
            return mapper.Map<LoanListItemDto[]>(list);
        }

        public async Task<LoanPaymentResultDto> PayAsync(Guid loanId, LoanPaymentRequestDto request, CancellationToken ct = default)
        {
            var loan = await loans.GetByIdWithCustomerAsync(loanId, ct);
            if (loan == null) throw new InvalidOperationException("Loan not found.");

            if (loan.Status == LoanStatus.Paid)
                throw new InvalidOperationException("Loan is already paid.");

            if (request.Amount <= 0)
                throw new InvalidOperationException("Payment amount must be greater than 0.");

            if (request.Amount > loan.CurrentBalance)
                throw new InvalidOperationException("Payment amount cannot exceed current balance.");

            var prev = loan.CurrentBalance;
            loan.CurrentBalance -= request.Amount;

            if (loan.CurrentBalance == 0)
                loan.Status = LoanStatus.Paid;

            await uow.SaveChangesAsync(ct);

            return new LoanPaymentResultDto
            {
                LoanId = loan.Id,
                PreviousBalance = prev,
                NewBalance = loan.CurrentBalance,
                Status = loan.Status.ToString().ToLowerInvariant()
            };
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Application.DTOs
{
    // POST /loans
    public class CreateLoanRequestDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }

    // GET /loans (tabla)
    public class LoanListItemDto
    {
        public Guid Id { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; }

        public CustomerDto Customer { get; set; } = default!;
    }

    // GET /loans/{id} (detalle)
    public class LoanDetailDto
    {
        public Guid Id { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; }

        public CustomerDto Customer { get; set; } = default!;
    }

    // POST /loans/{id}/payment
    public class LoanPaymentRequestDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }

    public class LoanPaymentResultDto
    {
        public Guid LoanId { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string Status { get; set; } = default!;
    }
}

using System;
using Fundo.Applications.WebApi.Domain.Enums;

namespace Fundo.Applications.WebApi.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;

namespace Fundo.Applications.WebApi.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = default!;
        public string? Email { get; set; }
        public List<Loan> Loans { get; set; } = new();
    }
}

using System;

namespace Fundo.Applications.WebApi.Application.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? Email { get; set; }
    }
}

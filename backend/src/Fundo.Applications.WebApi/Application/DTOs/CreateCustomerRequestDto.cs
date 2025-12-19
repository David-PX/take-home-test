namespace Fundo.Applications.WebApi.Application.DTOs
{
    public class CreateCustomerRequestDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}

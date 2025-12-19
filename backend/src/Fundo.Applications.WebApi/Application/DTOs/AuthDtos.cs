using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Application.DTOs
{
    public class AuthRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        // ya viene hasheado desde el frontend
        [Required, MinLength(20)]
        public string PasswordHash { get; set; } = default!;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;
    }
}

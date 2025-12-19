using System;

namespace Fundo.Applications.WebApi.Infrastructure.Persistence
{
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
    }
}

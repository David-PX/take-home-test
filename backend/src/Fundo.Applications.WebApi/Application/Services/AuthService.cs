using System;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Infrastructure.Auth;
using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;

namespace Fundo.Applications.WebApi.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository users;
        private readonly IUnitOfWork uow;
        private readonly JwtTokenService jwt;

        public AuthService(IUserRepository users, IUnitOfWork uow, JwtTokenService jwt)
        {
            this.users = users;
            this.uow = uow;
            this.jwt = jwt;
        }

        public async Task<AuthResponseDto> RegisterAsync(AuthRequestDto request, CancellationToken ct = default)
        {
            var exists = await users.ExistsByEmailAsync(request.Email, ct);
            if (exists) throw new InvalidOperationException("Email already registered.");

            var user = new AppUser
            {
                Email = request.Email,
                PasswordHash = request.PasswordHash
            };

            await users.AddAsync(user, ct);
            await uow.SaveChangesAsync(ct);

            return new AuthResponseDto { AccessToken = jwt.CreateToken(user.Id.ToString(), user.Email) };
        }

        public async Task<AuthResponseDto> LoginAsync(AuthRequestDto request, CancellationToken ct = default)
        {
            var user = await users.GetByEmailAsync(request.Email, ct);
            if (user == null) throw new InvalidOperationException("Invalid credentials.");

            if (!string.Equals(user.PasswordHash, request.PasswordHash, StringComparison.Ordinal))
                throw new InvalidOperationException("Invalid credentials.");

            return new AuthResponseDto { AccessToken = jwt.CreateToken(user.Id.ToString(), user.Email) };
        }
    }
}

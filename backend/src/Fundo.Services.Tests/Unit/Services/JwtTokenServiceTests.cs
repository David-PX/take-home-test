using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Fundo.Applications.WebApi.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Fundo.Services.Tests.Unit.Services
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public void CreateToken_WhenCalled_ShouldReturnJwtWithExpectedClaims()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Fundo",
                ["Jwt:Audience"] = "Fundo",
                ["Jwt:Key"] = "SUPER_LONG_SECRET_KEY_CHANGE_ME_32+_CHARS"
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var jwtService = new JwtTokenService(config);

            var userId = Guid.NewGuid().ToString();
            var email = "test@test.com";

            var token = jwtService.CreateToken(userId, email);

            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Issuer.Should().Be("Fundo");
            jwt.Audiences.Should().Contain("Fundo");

            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        }

        [Fact]
        public void CreateToken_WhenCalled_ShouldExpireInFuture()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Fundo",
                ["Jwt:Audience"] = "Fundo",
                ["Jwt:Key"] = "SUPER_LONG_SECRET_KEY_CHANGE_ME_32+_CHARS"
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var jwtService = new JwtTokenService(config);

            var token = jwtService.CreateToken(Guid.NewGuid().ToString(), "x@x.com");

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
        }
    }
}

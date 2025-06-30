using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace InnoShop.Users.Tests.Unit.Infrastructure.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public TokenServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SecretKey = "supersecretkeyforsigning1234567890",
                TokenLifetimeMinutes = 60
            };

            var options = Options.Create(_jwtSettings);
            _tokenService = new TokenService(options);
        }

        [Fact]
        public void GenerateAccessToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = Domain.Enums.UserRole.User
            };

            // Act
            var tokenString = _tokenService.GenerateAccessToken(user);

            // Assert
            Assert.False(string.IsNullOrEmpty(tokenString));

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal(_jwtSettings.Issuer, token.Issuer);
            Assert.Equal(_jwtSettings.Audience, token.Audiences.First());
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(token.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == user.Role.ToString());
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnNonEmptyBase64String()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrEmpty(refreshToken));

            // Проверим, что это валидная base64 строка
            var buffer = Convert.FromBase64String(refreshToken);
            Assert.Equal(32, buffer.Length);
        }
    }
}

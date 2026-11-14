using Auth.Configuration;
using DataAccess.Data;
using DataAccess.Data.UserRepo;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Exceptions;

namespace Auth.Services
{
    using LoginDto = Dto.LoginDto;
    using RegisterDto = Dto.RegisterDto;
    public class AuthService(IUserRepository userRepository, IOptions<JwtConfiguration> jwtConfig) : IAuthService
    {
        private static readonly string UnknownUserPasswordHash =
            BCrypt.Net.BCrypt.HashPassword("__unknown_user_login__");

        public async Task<string> LoginAsync(LoginDto loginDto, CancellationToken cancellation = default)
        {
            var user = await userRepository.GetByNameAsync(loginDto.Name, cancellation);

            var passwordHash = user?.PasswordHash ?? UnknownUserPasswordHash;
            var passwordMatches = BCrypt.Net.BCrypt.Verify(loginDto.Password, passwordHash);

            if (user is null || !passwordMatches)
                throw new UnauthorizedException("Invalid username or password");

            var config = jwtConfig.Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: config.Issuer,
                audience: config.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(config.ExpiresInMinutes),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellation = default)
        {
            var existing = await userRepository.GetByNameAsync(registerDto.Name, cancellation);
            if (existing != null)
                throw new ConflictException("User with this name already exists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            try
            {
                await userRepository.CreateAsync(new UserModel
                {
                    Name = registerDto.Name,
                    PasswordHash = passwordHash
                }, cancellation);
            }
            catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation("IX_Users_Name"))
            {

                throw new ConflictException("User with this name already exists");
            }
        }
    }
}

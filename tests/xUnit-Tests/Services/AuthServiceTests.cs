using System.IdentityModel.Tokens.Jwt;
using Auth.Configuration;
using Auth.Dto;
using Auth.Services;
using DataAccess.Models;
using Microsoft.Extensions.Options;
using Shared.Exceptions;

namespace XUnit_Tests.Services;

public class AuthServiceTests
{
    private const string Password = "password12345";

    private static readonly JwtConfiguration Configuration = new()
    {
        SecretKey = "unit-test-secret-key-1234567890-abcdefghij",
        Issuer = "TodoApi",
        Audience = "TodoApiClients",
        ExpiresInMinutes = 60
    };

    private readonly FakeUserRepository _repository = new();
    private readonly AuthService _service;

    public AuthServiceTests() => _service = new AuthService(_repository, Options.Create(Configuration));

    [Fact]
    public async Task LoginAsync_ReturnsTokenWithUserClaims()
    {
        var user = AddUser("alice");

        var token = await _service.LoginAsync(new LoginDto("alice", Password));
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(Configuration.Issuer, jwt.Issuer);
        Assert.Contains(Configuration.Audience, jwt.Audiences);
        Assert.Contains(jwt.Claims, claim => claim.Value == user.Id.ToString());
        Assert.Contains(jwt.Claims, claim => claim.Value == "alice");
        Assert.Contains(jwt.Claims, claim => claim.Type == JwtRegisteredClaimNames.Jti && claim.Value.Length > 0);
        Assert.InRange(jwt.ValidTo, DateTime.UtcNow.AddMinutes(55), DateTime.UtcNow.AddMinutes(61));
    }

    [Fact]
    public async Task LoginAsync_GeneratesUniqueTokenPerCall()
    {
        AddUser("alice");

        var first = new JwtSecurityTokenHandler()
            .ReadJwtToken(await _service.LoginAsync(new LoginDto("alice", Password)));
        var second = new JwtSecurityTokenHandler()
            .ReadJwtToken(await _service.LoginAsync(new LoginDto("alice", Password)));

        var firstJti = first.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;
        var secondJti = second.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;

        Assert.NotEqual(firstJti, secondJti);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorized()
    {
        AddUser("alice");

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _service.LoginAsync(new LoginDto("alice", "wrong-password")));
    }

    [Fact]
    public async Task LoginAsync_UnknownUser_ThrowsUnauthorized()
    {
        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _service.LoginAsync(new LoginDto("nobody", Password)));
    }

    [Fact]
    public async Task RegisterAsync_NewUser_StoresBcryptHash()
    {
        await _service.RegisterAsync(new RegisterDto("bob", Password));

        var user = Assert.Single(_repository.Users);
        Assert.Equal("bob", user.Name);
        Assert.NotEqual(Password, user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_DuplicateName_ThrowsConflict()
    {
        AddUser("alice");

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.RegisterAsync(new RegisterDto("alice", Password)));

        Assert.Equal(0, _repository.CreateCalls);
    }

    private UserModel AddUser(string name)
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password, workFactor: 4)
        };
        _repository.Users.Add(user);
        return user;
    }
}

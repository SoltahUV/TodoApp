using Auth.Dto;
using Auth.Services;

namespace IntegrationTests;

public class FakeAuthService : IAuthService
{
    public Task<string> LoginAsync(LoginDto loginDto, CancellationToken cancellation = default)
    {
        return Task.FromResult("fake-token");
    }

    public Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }
}

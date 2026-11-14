using Auth.Dto;
using IntegrationTests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Returns204()
    {
        var response = await _client.PostAsJsonAsync("/auth/register", new RegisterDto("newuser", "password12345"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Login_Returns200_WithToken()
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new LoginDto("user", "password12345"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrWhiteSpace(token.GetString()));
    }
}

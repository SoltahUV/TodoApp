using BusinessLogic.Dto;
using DataAccess.Models;
using IntegrationTests;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Controllers;

public class TasksControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TasksControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuth.Scheme, "fake-token");
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Create_Returns201_WithLocationAndDto()
    {
        FakeTaskService.Clear();
        var response = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskDto("Task", "Description"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/tasks/", response.Headers.Location!.OriginalString);

        var dto = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        Assert.NotNull(dto);
        Assert.Equal("Task", dto.Name);
        Assert.Equal("Description", dto.Description);
        Assert.NotEqual(Guid.Empty, dto.ItemId);
    }

    [Fact]
    public async Task GetAll_ReturnsTasks()
    {
        FakeTaskService.Clear();
        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskDto("Task 1", "Desc 1"));
        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskDto("Task 2", "Desc 2"));

        var response = await _client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dtos = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPage()
    {
        FakeTaskService.Clear();
        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskDto("One", null));
        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskDto("Two", null));
        await _client.PostAsJsonAsync("/api/tasks", new CreateTaskDto("Three", null));

        var first = await _client.GetAsync("/api/tasks?skip=0&take=2");
        var second = await _client.GetAsync("/api/tasks?skip=2&take=2");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var firstDtos = await first.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        var secondDtos = await second.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);

        Assert.Equal(2, firstDtos!.Count);
        Assert.Single(secondDtos!);
        Assert.DoesNotContain(
            secondDtos!.Select(d => d.ItemId),
            id => firstDtos.Select(d => d.ItemId).Contains(id));
    }

    [Fact]
    public async Task GetAll_NegativeSkip_Returns400()
    {
        var response = await _client.GetAsync("/api/tasks?skip=-1");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ZeroTake_Returns400()
    {
        var response = await _client.GetAsync("/api/tasks?take=0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ExcessiveTake_Returns400()
    {
        var response = await _client.GetAsync("/api/tasks?take=200");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/tasks/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Unauthorized_WithoutToken_Returns401()
    {
        var clientNoAuth = new CustomWebApplicationFactory().CreateClient();
        var response = await clientNoAuth.GetAsync("/api/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

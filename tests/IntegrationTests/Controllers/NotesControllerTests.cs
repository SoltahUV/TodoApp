using BusinessLogic.Dto;
using IntegrationTests;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Controllers;

public class NotesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public NotesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuth.Scheme, "fake-token");
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task Create_Returns201_WithLocationAndDto()
    {
        FakeNoteService.Clear();
        var response = await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto("Note", "Text"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/notes/", response.Headers.Location!.OriginalString);

        var dto = await response.Content.ReadFromJsonAsync<NoteDto>(JsonOptions);
        Assert.NotNull(dto);
        Assert.Equal("Note", dto.Name);
        Assert.Equal("Text", dto.Text);
        Assert.NotEqual(Guid.Empty, dto.ItemId);
    }

    [Fact]
    public async Task GetAll_ReturnsNotes()
    {
        FakeNoteService.Clear();
        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto("Note 1", "Text 1"));
        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto("Note 2", "Text 2"));

        var response = await _client.GetAsync("/api/notes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dtos = await response.Content.ReadFromJsonAsync<List<NoteDto>>(JsonOptions);
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Count);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPage()
    {
        FakeNoteService.Clear();
        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto("One", null));
        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto("Two", null));
        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto("Three", null));

        var first = await _client.GetAsync("/api/notes?skip=0&take=2");
        var second = await _client.GetAsync("/api/notes?skip=2&take=2");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var firstDtos = await first.Content.ReadFromJsonAsync<List<NoteDto>>(JsonOptions);
        var secondDtos = await second.Content.ReadFromJsonAsync<List<NoteDto>>(JsonOptions);

        Assert.Equal(2, firstDtos!.Count);
        Assert.Single(secondDtos!);
        Assert.DoesNotContain(
            secondDtos!.Select(d => d.ItemId),
            id => firstDtos.Select(d => d.ItemId).Contains(id));
    }

    [Fact]
    public async Task GetAll_NegativeSkip_Returns400()
    {
        var response = await _client.GetAsync("/api/notes?skip=-1");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ExcessiveTake_Returns400()
    {
        var response = await _client.GetAsync("/api/notes?take=200");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/notes/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Unauthorized_WithoutToken_Returns401()
    {
        var clientNoAuth = new CustomWebApplicationFactory().CreateClient();
        var response = await clientNoAuth.GetAsync("/api/notes");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

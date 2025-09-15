using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PathfinderSpellDB.Services;
using PathfinderSpellDB.Models;
using System.Net;

namespace PathfinderSpellDB.Tests.Integration;

public class ServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HomePage_ShouldLoadSuccessfully()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Pathfinder 2E Spell Database");
    }

    [Fact]
    public async Task BookmarksPage_ShouldLoadSuccessfully()
    {
        // Act
        var response = await _client.GetAsync("/bookmarks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("My Bookmarks");
    }

    [Fact]
    public async Task AboutPage_ShouldLoadSuccessfully()
    {
        // Act
        var response = await _client.GetAsync("/about");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("About Pathfinder 2E Spell Database");
    }

    [Fact]
    public async Task QuickRefPage_ShouldLoadSuccessfully()
    {
        // Act
        var response = await _client.GetAsync("/quickref");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Quick Reference");
    }

    [Fact]
    public async Task Services_ShouldBeRegistered()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var spellService = scope.ServiceProvider.GetService<ISpellService>();
        var bookmarkService = scope.ServiceProvider.GetService<IBookmarkService>();

        // Assert
        spellService.Should().NotBeNull();
        bookmarkService.Should().NotBeNull();
    }

    [Fact]
    public async Task SpellService_ShouldLoadData()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var spellService = scope.ServiceProvider.GetRequiredService<ISpellService>();

        // Act
        var spells = await spellService.GetAllSpellsAsync();
        var spellTypes = await spellService.GetSpellTypesAsync();

        // Assert
        spells.Should().NotBeEmpty();
        spellTypes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BookmarkService_ShouldCreateDefaultList()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var bookmarkService = scope.ServiceProvider.GetRequiredService<IBookmarkService>();

        // Act
        var lists = await bookmarkService.GetBookmarkListsAsync();
        var activeList = await bookmarkService.GetActiveBookmarkListAsync();

        // Assert
        lists.Should().NotBeEmpty();
        activeList.Should().NotBeNull();
        activeList!.Name.Should().Be("My Spells");
    }
}
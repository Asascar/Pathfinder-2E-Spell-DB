using Xunit;
using Moq;
using FluentAssertions;
using PathfinderSpellDB.Services;
using PathfinderSpellDB.Models;
using Microsoft.AspNetCore.Hosting;

namespace PathfinderSpellDB.Tests.Unit;

public class BookmarkServiceTests
{
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly BookmarkService _bookmarkService;

    public BookmarkServiceTests()
    {
        _mockEnvironment = new Mock<IWebHostEnvironment>();
        _mockEnvironment.Setup(e => e.WebRootPath).Returns("/wwwroot");
        _bookmarkService = new BookmarkService(_mockEnvironment.Object);
    }

    [Fact]
    public async Task CreateBookmarkListAsync_ShouldCreateNewList()
    {
        // Arrange
        var listName = "Test List";
        var isVancian = true;

        // Act
        var result = await _bookmarkService.CreateBookmarkListAsync(listName, isVancian);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(listName);
        result.Vancian.Should().Be(isVancian);
        result.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AddSpellToBookmarkListAsync_ShouldAddSpell()
    {
        // Arrange
        var list = await _bookmarkService.CreateBookmarkListAsync("Test List", false);
        var spellName = "Fireball";

        // Act
        await _bookmarkService.AddSpellToBookmarkListAsync(list.Id, spellName);

        // Assert
        var updatedList = await _bookmarkService.GetBookmarkListsAsync();
        var targetList = updatedList.First(l => l.Id == list.Id);
        targetList.Spells.Should().ContainKey(spellName);
    }

    [Fact]
    public async Task IsSpellBookmarkedAsync_ShouldReturnCorrectStatus()
    {
        // Arrange
        var list = await _bookmarkService.CreateBookmarkListAsync("Test List", false);
        var spellName = "Fireball";
        await _bookmarkService.AddSpellToBookmarkListAsync(list.Id, spellName);
        await _bookmarkService.SetActiveBookmarkListAsync(list.Id);

        // Act
        var result = await _bookmarkService.IsSpellBookmarkedAsync(spellName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveSpellFromBookmarkListAsync_ShouldRemoveSpell()
    {
        // Arrange
        var list = await _bookmarkService.CreateBookmarkListAsync("Test List", false);
        var spellName = "Fireball";
        await _bookmarkService.AddSpellToBookmarkListAsync(list.Id, spellName);

        // Act
        await _bookmarkService.RemoveSpellFromBookmarkListAsync(list.Id, spellName);

        // Assert
        var updatedList = await _bookmarkService.GetBookmarkListsAsync();
        var targetList = updatedList.First(l => l.Id == list.Id);
        targetList.Spells.Should().NotContainKey(spellName);
    }

    [Fact]
    public async Task UpdateBookmarkSpellAsync_ShouldUpdateSpellData()
    {
        // Arrange
        var list = await _bookmarkService.CreateBookmarkListAsync("Test List", true);
        var spellName = "Fireball";
        await _bookmarkService.AddSpellToBookmarkListAsync(list.Id, spellName);
        
        var updatedBookmarkSpell = new BookmarkSpell
        {
            VancianPrep = 3,
            VancianCast = 1,
            Alt = new List<string> { "Heightened" }
        };

        // Act
        await _bookmarkService.UpdateBookmarkSpellAsync(list.Id, spellName, updatedBookmarkSpell);

        // Assert
        var updatedList = await _bookmarkService.GetBookmarkListsAsync();
        var targetList = updatedList.First(l => l.Id == list.Id);
        var bookmarkSpell = targetList.Spells[spellName];
        bookmarkSpell.VancianPrep.Should().Be(3);
        bookmarkSpell.VancianCast.Should().Be(1);
        bookmarkSpell.Alt.Should().Contain("Heightened");
    }

    [Fact]
    public async Task DeleteBookmarkListAsync_ShouldRemoveList()
    {
        // Arrange
        var list = await _bookmarkService.CreateBookmarkListAsync("Test List", false);
        var initialCount = (await _bookmarkService.GetBookmarkListsAsync()).Count;

        // Act
        await _bookmarkService.DeleteBookmarkListAsync(list.Id);

        // Assert
        var updatedLists = await _bookmarkService.GetBookmarkListsAsync();
        updatedLists.Should().HaveCount(initialCount - 1);
        updatedLists.Should().NotContain(l => l.Id == list.Id);
    }

    [Fact]
    public async Task GetActiveBookmarkListAsync_ShouldReturnActiveList()
    {
        // Arrange
        var list1 = await _bookmarkService.CreateBookmarkListAsync("List 1", false);
        var list2 = await _bookmarkService.CreateBookmarkListAsync("List 2", false);
        await _bookmarkService.SetActiveBookmarkListAsync(list2.Id);

        // Act
        var result = await _bookmarkService.GetActiveBookmarkListAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(list2.Id);
        result.Name.Should().Be("List 2");
    }
}
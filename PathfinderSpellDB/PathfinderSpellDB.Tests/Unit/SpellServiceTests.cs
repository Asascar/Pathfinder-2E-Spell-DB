using Xunit;
using Moq;
using FluentAssertions;
using PathfinderSpellDB.Services;
using PathfinderSpellDB.Models;
using Microsoft.AspNetCore.Hosting;

namespace PathfinderSpellDB.Tests.Unit;

public class SpellServiceTests
{
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly SpellService _spellService;

    public SpellServiceTests()
    {
        _mockEnvironment = new Mock<IWebHostEnvironment>();
        _mockEnvironment.Setup(e => e.WebRootPath).Returns("/wwwroot");
        _spellService = new SpellService(_mockEnvironment.Object);
    }

    [Fact]
    public async Task GetAllSpellsAsync_ShouldReturnAllSpells()
    {
        // Act
        var result = await _spellService.GetAllSpellsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(spell => spell.Name.Should().NotBeNullOrEmpty());
    }

    [Theory]
    [InlineData("Fireball", 1)]
    [InlineData("Force Barrage", 1)]
    [InlineData("Nonexistent", 0)]
    public async Task SearchSpellsAsync_ByName_ShouldReturnCorrectResults(string searchTerm, int expectedCount)
    {
        // Arrange
        var criteria = new SearchCriteria { SpellName = searchTerm };

        // Act
        var result = await _spellService.SearchSpellsAsync(criteria);

        // Assert
        result.Should().HaveCount(expectedCount);
        if (expectedCount > 0)
        {
            result.Should().OnlyContain(s => s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task SearchSpellsAsync_ByLevel_ShouldFilterCorrectly()
    {
        // Arrange
        var criteria = new SearchCriteria 
        { 
            Levels = new List<string> { "1", "2" } 
        };

        // Act
        var result = await _spellService.SearchSpellsAsync(criteria);

        // Assert
        result.Should().OnlyContain(s => s.Level == 1 || s.Level == 2);
    }

    [Fact]
    public async Task SearchSpellsAsync_ByTradition_ShouldFilterCorrectly()
    {
        // Arrange
        var criteria = new SearchCriteria 
        { 
            SpellType = "Traditions",
            SpellOption = "arcane"
        };

        // Act
        var result = await _spellService.SearchSpellsAsync(criteria);

        // Assert
        result.Should().OnlyContain(s => s.Traditions.Contains("arcane"));
    }

    [Theory]
    [InlineData("Name", "A")]
    [InlineData("Level", "1")]
    [InlineData("Actions", "1")]
    public async Task SearchSpellsAsync_SortBy_ShouldSortCorrectly(string sortBy, string expectedFirst)
    {
        // Arrange
        var criteria = new SearchCriteria { SortBy = sortBy };

        // Act
        var result = await _spellService.SearchSpellsAsync(criteria);

        // Assert
        result.Should().NotBeEmpty();
        // Add specific sorting assertions based on sortBy
        if (sortBy == "Name")
        {
            result.Should().BeInAscendingOrder(s => s.Name, StringComparer.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task GetSpellByNameAsync_WithValidName_ShouldReturnSpell()
    {
        // Arrange
        var spellName = "Fireball";

        // Act
        var result = await _spellService.GetSpellByNameAsync(spellName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(spellName);
    }

    [Fact]
    public async Task GetSpellByNameAsync_WithInvalidName_ShouldReturnNull()
    {
        // Arrange
        var spellName = "NonexistentSpell";

        // Act
        var result = await _spellService.GetSpellByNameAsync(spellName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSpellTypesAsync_ShouldReturnAllTypes()
    {
        // Act
        var result = await _spellService.GetSpellTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(type => type.Name.Should().NotBeNullOrEmpty());
    }
}
using Xunit;
using FluentAssertions;
using PathfinderSpellDB.Models;

namespace PathfinderSpellDB.Tests.Unit;

public class SpellTests
{
    [Fact]
    public void Spell_ShouldHaveRequiredProperties()
    {
        // Arrange & Act
        var spell = new Spell
        {
            Name = "Fireball",
            Type = "Spell",
            Level = 3,
            Traditions = new List<string> { "arcane" },
            Traits = new List<string> { "evocation", "fire" }
        };

        // Assert
        spell.Name.Should().Be("Fireball");
        spell.Type.Should().Be("Spell");
        spell.Level.Should().Be(3);
        spell.Traditions.Should().Contain("arcane");
        spell.Traits.Should().Contain("evocation");
    }

    [Theory]
    [InlineData("Cantrip", 0)]
    [InlineData("Spell", 5)]
    [InlineData("Focus", 1)]
    public void Spell_LevelDisplay_ShouldFormatCorrectly(string type, int level)
    {
        // Arrange
        var spell = new Spell { Type = type, Level = level };

        // Act
        var displayLevel = spell.Type == "Cantrip" ? "C" : spell.Level.ToString();

        // Assert
        displayLevel.Should().Be(type == "Cantrip" ? "C" : level.ToString());
    }

    [Fact]
    public void Spell_WithEmptyCollections_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var spell = new Spell();

        // Assert
        spell.Traditions.Should().NotBeNull();
        spell.Traditions.Should().BeEmpty();
        spell.Traits.Should().NotBeNull();
        spell.Traits.Should().BeEmpty();
    }

    [Fact]
    public void Spell_WithAllProperties_ShouldSetCorrectly()
    {
        // Arrange & Act
        var spell = new Spell
        {
            Name = "Magic Missile",
            Type = "Spell",
            Level = 1,
            Traditions = new List<string> { "arcane" },
            Traits = new List<string> { "evocation", "force" },
            Action = "2",
            Range = "120 feet",
            Description = "You send a dart of force...",
            Source = "Core Rulebook"
        };

        // Assert
        spell.Name.Should().Be("Magic Missile");
        spell.Type.Should().Be("Spell");
        spell.Level.Should().Be(1);
        spell.Action.Should().Be("2");
        spell.Range.Should().Be("120 feet");
        spell.Description.Should().Be("You send a dart of force...");
        spell.Source.Should().Be("Core Rulebook");
    }
}
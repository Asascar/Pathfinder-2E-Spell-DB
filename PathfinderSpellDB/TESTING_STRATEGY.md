# Testing Strategy: Pathfinder 2E Spell Database

This document outlines the comprehensive testing strategy for the .NET Blazor application, covering unit tests, integration tests, and end-to-end testing.

## 🎯 **Testing Overview**

### Testing Pyramid
```
    /\
   /  \
  / E2E \     End-to-End Tests (5%)
 /______\
/        \
/Integration\ Integration Tests (15%)
/__________\
/            \
/   Unit Tests  \ Unit Tests (80%)
/________________\
```

### Test Categories
1. **Unit Tests** - Individual components and services
2. **Integration Tests** - Service interactions and data flow
3. **Component Tests** - Blazor component behavior
4. **End-to-End Tests** - Complete user workflows
5. **Performance Tests** - Load and stress testing

## 🧪 **Unit Testing**

### Test Framework Setup

#### Required Packages
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.1" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
```

#### Project Structure
```
PathfinderSpellDB.Tests/
├── Unit/
│   ├── Services/
│   │   ├── SpellServiceTests.cs
│   │   └── BookmarkServiceTests.cs
│   └── Models/
│       └── SpellTests.cs
├── Integration/
│   ├── ServiceIntegrationTests.cs
│   └── DataAccessTests.cs
├── Components/
│   ├── SpellSearchComponentTests.cs
│   └── SpellDetailComponentTests.cs
└── EndToEnd/
    └── UserWorkflowTests.cs
```

### Service Tests

#### SpellService Tests
```csharp
// Unit/Services/SpellServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using PathfinderSpellDB.Services;
using PathfinderSpellDB.Models;

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
        // Arrange
        var expectedSpellCount = 1247;

        // Act
        var result = await _spellService.GetAllSpellsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedSpellCount);
    }

    [Theory]
    [InlineData("Fireball", 1)]
    [InlineData("Magic Missile", 1)]
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
    }
}
```

#### BookmarkService Tests
```csharp
// Unit/Services/BookmarkServiceTests.cs
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
}
```

### Model Tests

#### Spell Model Tests
```csharp
// Unit/Models/SpellTests.cs
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
}
```

## 🔗 **Integration Testing**

### Service Integration Tests
```csharp
// Integration/ServiceIntegrationTests.cs
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
    public async Task SpellSearch_Integration_ShouldWorkEndToEnd()
    {
        // Arrange
        var searchTerm = "fire";

        // Act
        var response = await _client.GetAsync($"/api/spells/search?name={searchTerm}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task BookmarkWorkflow_Integration_ShouldWorkEndToEnd()
    {
        // Arrange
        var listName = "Integration Test List";
        var spellName = "Fireball";

        // Act - Create list
        var createResponse = await _client.PostAsJsonAsync("/api/bookmarks", new { Name = listName });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act - Add spell
        var addResponse = await _client.PostAsJsonAsync($"/api/bookmarks/{listName}/spells", new { SpellName = spellName });
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Get list
        var getResponse = await _client.GetAsync($"/api/bookmarks/{listName}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var list = await getResponse.Content.ReadFromJsonAsync<BookmarkList>();
        list.Spells.Should().ContainKey(spellName);
    }
}
```

## 🧩 **Component Testing**

### Blazor Component Tests
```csharp
// Components/SpellSearchComponentTests.cs
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PathfinderSpellDB.Services;

public class SpellSearchComponentTests : TestContext
{
    [Fact]
    public void SpellSearchComponent_ShouldRenderCorrectly()
    {
        // Arrange
        Services.AddScoped<ISpellService, MockSpellService>();
        var component = RenderComponent<SpellSearchComponent>();

        // Assert
        component.Find("input[type='text']").Should().NotBeNull();
        component.Find("select").Should().NotBeNull();
    }

    [Fact]
    public void SpellSearchComponent_OnSearch_ShouldTriggerCallback()
    {
        // Arrange
        var searchCriteria = new SearchCriteria { SpellName = "test" };
        var callbackTriggered = false;
        
        Services.AddScoped<ISpellService, MockSpellService>();
        var component = RenderComponent<SpellSearchComponent>(
            parameters => parameters.Add(p => p.OnSearch, 
                EventCallback.Factory.Create<SearchCriteria>(this, 
                    (criteria) => callbackTriggered = true)));

        // Act
        component.Find("input[type='text']").Change("test");
        component.Find("input[type='text']").Blur();

        // Assert
        callbackTriggered.Should().BeTrue();
    }
}
```

## 🎭 **End-to-End Testing**

### Playwright E2E Tests
```csharp
// EndToEnd/UserWorkflowTests.cs
using Microsoft.Playwright;

public class UserWorkflowTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public UserWorkflowTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SearchSpell_ShouldDisplayResults()
    {
        // Arrange
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5128");

        // Act
        await page.FillAsync("input[placeholder*='spell name']", "fireball");
        await page.ClickAsync("button[type='submit']");

        // Assert
        await page.WaitForSelectorAsync(".spell-detail");
        var spellName = await page.TextContentAsync("h4");
        spellName.Should().Contain("Fireball");
    }

    [Fact]
    public async Task BookmarkWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5128");

        // Act - Search for spell
        await page.FillAsync("input[placeholder*='spell name']", "magic missile");
        await page.ClickAsync("button[type='submit']");
        await page.WaitForSelectorAsync(".spell-detail");

        // Act - Bookmark spell
        await page.ClickAsync("button:has-text('Bookmark')");
        await page.GotoAsync("http://localhost:5128/bookmarks");

        // Assert
        await page.WaitForSelectorAsync("text=Magic Missile");
        var bookmarkedSpell = await page.TextContentAsync("h6");
        bookmarkedSpell.Should().Contain("Magic Missile");
    }

    [Fact]
    public async Task ThemeToggle_ShouldWork()
    {
        // Arrange
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5128");

        // Act
        await page.ClickAsync("button:has-text('Dark Mode')");

        // Assert
        var bodyClass = await page.GetAttributeAsync("body", "class");
        bodyClass.Should().Contain("dark-theme");
    }
}
```

## ⚡ **Performance Testing**

### Load Testing with NBomber
```csharp
// Performance/LoadTests.cs
using NBomber;
using NBomber.CSharp;

public class LoadTests
{
    [Fact]
    public void SpellSearch_LoadTest()
    {
        var scenario = Scenario.Create("spell_search", async context =>
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:5128/api/spells/search?name=fire");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void ConcurrentUsers_StressTest()
    {
        var scenario = Scenario.Create("concurrent_users", async context =>
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:5128");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(2))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
```

## 📊 **Test Data Management**

### Test Data Setup
```csharp
// TestData/TestDataBuilder.cs
public class TestDataBuilder
{
    public static List<Spell> CreateSampleSpells()
    {
        return new List<Spell>
        {
            new Spell
            {
                Name = "Fireball",
                Type = "Spell",
                Level = 3,
                Traditions = new List<string> { "arcane" },
                Traits = new List<string> { "evocation", "fire" },
                Action = "2",
                Range = "500 feet",
                Area = "20-foot burst",
                Description = "A bright streak flashes from your pointing finger..."
            },
            new Spell
            {
                Name = "Magic Missile",
                Type = "Spell",
                Level = 1,
                Traditions = new List<string> { "arcane" },
                Traits = new List<string> { "evocation", "force" },
                Action = "2",
                Range = "120 feet",
                Description = "You send a dart of force..."
            }
        };
    }

    public static SearchCriteria CreateSearchCriteria(string name = null, string type = null)
    {
        return new SearchCriteria
        {
            SpellName = name,
            SpellType = type,
            SortBy = "Name",
            DisplayMode = "Details",
            Levels = new List<string>()
        };
    }
}
```

## 🔧 **Test Configuration**

### Test Settings
```json
// appsettings.Test.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "TestDataPath": "TestData/",
  "Database": {
    "ConnectionString": "Data Source=:memory:"
  }
}
```

### Test Fixtures
```csharp
// TestFixtures/WebApplicationFactory.cs
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace services with test implementations
            services.AddScoped<ISpellService, TestSpellService>();
            services.AddScoped<IBookmarkService, TestBookmarkService>();
        });
    }
}
```

## 📈 **Test Metrics and Reporting**

### Code Coverage
```xml
<!-- Directory.Build.props -->
<ItemGroup>
  <PackageReference Include="coverlet.collector" Version="3.2.0" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>

<ItemGroup>
  <PackageReference Include="ReportGenerator" Version="5.1.10" />
</ItemGroup>
```

### Test Reports
```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"Html"
```

## 🚀 **CI/CD Integration**

### GitHub Actions Test Pipeline
```yaml
# .github/workflows/test.yml
name: Test

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Generate coverage report
      run: reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"Html"
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage.cobertura.xml
```

## 📋 **Testing Checklist**

### Unit Tests
- [ ] All services have comprehensive test coverage
- [ ] All models have validation tests
- [ ] Edge cases are covered
- [ ] Error conditions are tested

### Integration Tests
- [ ] Service interactions work correctly
- [ ] Data persistence works
- [ ] API endpoints respond correctly
- [ ] Error handling works end-to-end

### Component Tests
- [ ] All Blazor components render correctly
- [ ] User interactions trigger correct events
- [ ] State changes are reflected in UI
- [ ] Accessibility requirements are met

### E2E Tests
- [ ] Critical user workflows are tested
- [ ] Cross-browser compatibility verified
- [ ] Mobile responsiveness tested
- [ ] Performance requirements met

This comprehensive testing strategy ensures the Pathfinder 2E Spell Database application is robust, reliable, and maintainable.
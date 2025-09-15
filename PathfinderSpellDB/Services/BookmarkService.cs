using System.Text.Json;
using PathfinderSpellDB.Models;

namespace PathfinderSpellDB.Services;

public class BookmarkService : IBookmarkService
{
    private readonly IWebHostEnvironment _environment;
    private List<BookmarkList>? _bookmarkLists;
    private string? _activeListId;

    public BookmarkService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<List<BookmarkList>> GetBookmarkListsAsync()
    {
        if (_bookmarkLists == null)
        {
            await LoadBookmarkListsAsync();
        }
        return _bookmarkLists ?? new List<BookmarkList>();
    }

    public async Task<BookmarkList?> GetActiveBookmarkListAsync()
    {
        var lists = await GetBookmarkListsAsync();
        if (string.IsNullOrEmpty(_activeListId))
        {
            _activeListId = lists.FirstOrDefault()?.Id;
        }
        return lists.FirstOrDefault(l => l.Id == _activeListId);
    }

    public async Task SetActiveBookmarkListAsync(string listId)
    {
        _activeListId = listId;
        await SaveBookmarkListsAsync();
    }

    public async Task<BookmarkList> CreateBookmarkListAsync(string name, bool vancian = false)
    {
        var lists = await GetBookmarkListsAsync();
        var newList = new BookmarkList
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Vancian = vancian,
            Spells = new Dictionary<string, BookmarkSpell>()
        };
        
        lists.Add(newList);
        await SaveBookmarkListsAsync();
        return newList;
    }

    public async Task DeleteBookmarkListAsync(string listId)
    {
        var lists = await GetBookmarkListsAsync();
        var listToRemove = lists.FirstOrDefault(l => l.Id == listId);
        if (listToRemove != null)
        {
            lists.Remove(listToRemove);
            if (_activeListId == listId)
            {
                _activeListId = lists.FirstOrDefault()?.Id;
            }
            await SaveBookmarkListsAsync();
        }
    }

    public async Task AddSpellToBookmarkListAsync(string listId, string spellName)
    {
        var lists = await GetBookmarkListsAsync();
        var list = lists.FirstOrDefault(l => l.Id == listId);
        if (list != null && !list.Spells.ContainsKey(spellName))
        {
            list.Spells[spellName] = new BookmarkSpell();
            await SaveBookmarkListsAsync();
        }
    }

    public async Task RemoveSpellFromBookmarkListAsync(string listId, string spellName)
    {
        var lists = await GetBookmarkListsAsync();
        var list = lists.FirstOrDefault(l => l.Id == listId);
        if (list != null && list.Spells.ContainsKey(spellName))
        {
            list.Spells.Remove(spellName);
            await SaveBookmarkListsAsync();
        }
    }

    public async Task<bool> IsSpellBookmarkedAsync(string spellName)
    {
        var activeList = await GetActiveBookmarkListAsync();
        return activeList?.Spells.ContainsKey(spellName) ?? false;
    }

    public async Task UpdateBookmarkSpellAsync(string listId, string spellName, BookmarkSpell bookmarkSpell)
    {
        var lists = await GetBookmarkListsAsync();
        var list = lists.FirstOrDefault(l => l.Id == listId);
        if (list != null && list.Spells.ContainsKey(spellName))
        {
            list.Spells[spellName] = bookmarkSpell;
            await SaveBookmarkListsAsync();
        }
    }

    private async Task LoadBookmarkListsAsync()
    {
        var filePath = GetBookmarkListsFilePath();
        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            _bookmarkLists = JsonSerializer.Deserialize<List<BookmarkList>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else
        {
            _bookmarkLists = new List<BookmarkList>();
            // Create default list
            var defaultList = new BookmarkList
            {
                Id = Guid.NewGuid().ToString(),
                Name = "My Spells",
                Vancian = false,
                Spells = new Dictionary<string, BookmarkSpell>()
            };
            _bookmarkLists.Add(defaultList);
            await SaveBookmarkListsAsync();
        }
    }

    private async Task SaveBookmarkListsAsync()
    {
        if (_bookmarkLists != null)
        {
            var filePath = GetBookmarkListsFilePath();
            var json = JsonSerializer.Serialize(_bookmarkLists, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await File.WriteAllTextAsync(filePath, json);
        }
    }

    private string GetBookmarkListsFilePath()
    {
        var dataPath = GetDataDirectory();
        Directory.CreateDirectory(dataPath);
        return Path.Combine(dataPath, "bookmarks.json");
    }

    private string GetDataDirectory()
    {
        // For tests, use a temporary directory
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" || 
            AppDomain.CurrentDomain.FriendlyName.Contains("testhost"))
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "PathfinderSpellDB", "Data");
            return tempPath;
        }

        // Try multiple possible locations for the Data directory
        var possiblePaths = new[]
        {
            Path.Combine(_environment.WebRootPath, "..", "Data"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "Data"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Data")
        };

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path) || Directory.Exists(Path.GetDirectoryName(path) ?? ""))
            {
                return path;
            }
        }

        // If no directory found, return the first path
        return possiblePaths[0];
    }
}
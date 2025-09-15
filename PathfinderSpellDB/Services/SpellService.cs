using System.Text.Json;
using PathfinderSpellDB.Models;

namespace PathfinderSpellDB.Services;

public class SpellService : ISpellService
{
    private readonly IWebHostEnvironment _environment;
    private List<Spell>? _spells;
    private List<SpellType>? _spellTypes;

    public SpellService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<List<Spell>> GetAllSpellsAsync()
    {
        if (_spells == null)
        {
            await LoadSpellsAsync();
        }
        return _spells ?? new List<Spell>();
    }

    public async Task<List<SpellType>> GetSpellTypesAsync()
    {
        if (_spellTypes == null)
        {
            await LoadSpellTypesAsync();
        }
        return _spellTypes ?? new List<SpellType>();
    }

    public async Task<List<Spell>> SearchSpellsAsync(SearchCriteria criteria)
    {
        var spells = await GetAllSpellsAsync();
        var filteredSpells = spells.AsEnumerable();

        // Filter by name
        if (!string.IsNullOrEmpty(criteria.SpellName))
        {
            filteredSpells = filteredSpells.Where(s => 
                s.Name.Contains(criteria.SpellName, StringComparison.OrdinalIgnoreCase) ||
                (s.OldName != null && s.OldName.Contains(criteria.SpellName, StringComparison.OrdinalIgnoreCase)));
        }

        // Filter by spell type
        if (!string.IsNullOrEmpty(criteria.SpellType))
        {
            var spellTypes = await GetSpellTypesAsync();
            var spellType = spellTypes.FirstOrDefault(st => st.Name == criteria.SpellType);
            
            if (spellType != null)
            {
                filteredSpells = ApplySpellTypeFilter(filteredSpells, spellType, criteria.SpellOption);
            }
        }

        // Filter by levels
        if (criteria.Levels.Any())
        {
            filteredSpells = filteredSpells.Where(s =>
            {
                if (s.Type == "Cantrip")
                    return criteria.Levels.Contains("C");
                return criteria.Levels.Contains(s.Level.ToString());
            });
        }

        // Sort
        var sortedSpells = ApplySorting(filteredSpells, criteria.SortBy);

        return sortedSpells.ToList();
    }

    public async Task<Spell?> GetSpellByNameAsync(string name)
    {
        var spells = await GetAllSpellsAsync();
        return spells.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private async Task LoadSpellsAsync()
    {
        var filePath = Path.Combine(_environment.WebRootPath, "..", "Data", "spells.json");
        var json = await File.ReadAllTextAsync(filePath);
        _spells = JsonSerializer.Deserialize<List<Spell>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private async Task LoadSpellTypesAsync()
    {
        var filePath = Path.Combine(_environment.WebRootPath, "..", "Data", "spellTypes.json");
        var json = await File.ReadAllTextAsync(filePath);
        _spellTypes = JsonSerializer.Deserialize<List<SpellType>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private IEnumerable<Spell> ApplySpellTypeFilter(IEnumerable<Spell> spells, SpellType spellType, string spellOption)
    {
        return spellType.MatchBy switch
        {
            "array" => spells.Where(s => 
                !string.IsNullOrEmpty(spellOption) && 
                s.GetType().GetProperty(spellType.Match ?? "")?.GetValue(s) is List<string> list && 
                list.Contains(spellOption)),
            "value" => spells.Where(s => 
                !string.IsNullOrEmpty(spellOption) && 
                s.GetType().GetProperty(spellType.Match ?? "")?.GetValue(s)?.ToString() == spellOption),
            "contains" => spells.Where(s => 
                !string.IsNullOrEmpty(spellOption) && 
                s.GetType().GetProperty(spellType.Match ?? "")?.GetValue(s)?.ToString()?.Contains(spellOption, StringComparison.OrdinalIgnoreCase) == true),
            "filter" when spellType.Filter != null => spells.Where(s => 
                spellType.Filter.All(f => 
                    s.GetType().GetProperty(f.Key)?.GetValue(s) is List<string> list && 
                    list.Contains(f.Value))),
            _ => spells
        };
    }

    private IEnumerable<Spell> ApplySorting(IEnumerable<Spell> spells, string sortBy)
    {
        return sortBy switch
        {
            "Name" => spells.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase),
            "Level" => spells.OrderBy(s => s.Type == "Cantrip" ? 0 : s.Level)
                             .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase),
            "Actions" => spells.OrderBy(s => GetActionOrder(s.Action))
                               .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase),
            _ => spells
        };
    }

    private int GetActionOrder(string? action)
    {
        if (string.IsNullOrEmpty(action)) return 10;
        
        var actionOrder = new[] { "free", "reaction", "1", "2", "3" };
        var actionValue = action.Split(',')[0].Trim();
        var index = Array.IndexOf(actionOrder, actionValue);
        return index == -1 ? 10 : index;
    }
}
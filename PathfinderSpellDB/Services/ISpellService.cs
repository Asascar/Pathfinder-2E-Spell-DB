using PathfinderSpellDB.Models;

namespace PathfinderSpellDB.Services;

public interface ISpellService
{
    Task<List<Spell>> GetAllSpellsAsync();
    Task<List<SpellType>> GetSpellTypesAsync();
    Task<List<Spell>> SearchSpellsAsync(SearchCriteria criteria);
    Task<Spell?> GetSpellByNameAsync(string name);
}
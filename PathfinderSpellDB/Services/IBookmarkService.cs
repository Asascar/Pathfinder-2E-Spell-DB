using PathfinderSpellDB.Models;

namespace PathfinderSpellDB.Services;

public interface IBookmarkService
{
    Task<List<BookmarkList>> GetBookmarkListsAsync();
    Task<BookmarkList?> GetActiveBookmarkListAsync();
    Task SetActiveBookmarkListAsync(string listId);
    Task<BookmarkList> CreateBookmarkListAsync(string name, bool vancian = false);
    Task DeleteBookmarkListAsync(string listId);
    Task AddSpellToBookmarkListAsync(string listId, string spellName);
    Task RemoveSpellFromBookmarkListAsync(string listId, string spellName);
    Task<bool> IsSpellBookmarkedAsync(string spellName);
    Task UpdateBookmarkSpellAsync(string listId, string spellName, BookmarkSpell bookmarkSpell);
}
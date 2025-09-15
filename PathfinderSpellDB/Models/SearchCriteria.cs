namespace PathfinderSpellDB.Models;

public class SearchCriteria
{
    public string SpellName { get; set; } = string.Empty;
    public string SpellType { get; set; } = string.Empty;
    public string SpellOption { get; set; } = string.Empty;
    public string SortBy { get; set; } = "Level";
    public string DisplayMode { get; set; } = "Details";
    public List<string> Levels { get; set; } = new();
}
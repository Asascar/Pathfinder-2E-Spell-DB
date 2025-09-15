using System.Text.Json.Serialization;

namespace PathfinderSpellDB.Models;

public class BookmarkList
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("vancian")]
    public bool Vancian { get; set; }

    [JsonPropertyName("spells")]
    public Dictionary<string, BookmarkSpell> Spells { get; set; } = new();
}

public class BookmarkSpell
{
    [JsonPropertyName("vancianPrep")]
    public int VancianPrep { get; set; }

    [JsonPropertyName("vancianCast")]
    public int VancianCast { get; set; }

    [JsonPropertyName("alt")]
    public List<string> Alt { get; set; } = new();
}
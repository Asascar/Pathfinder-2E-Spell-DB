using System.Text.Json.Serialization;

namespace PathfinderSpellDB.Models;

public class SpellType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("matchBy")]
    public string MatchBy { get; set; } = string.Empty;

    [JsonPropertyName("match")]
    public string? Match { get; set; }

    [JsonPropertyName("sort")]
    public string? Sort { get; set; }

    [JsonPropertyName("allLabel")]
    public string? AllLabel { get; set; }

    [JsonPropertyName("options")]
    public List<SpellTypeOption>? Options { get; set; }

    [JsonPropertyName("filter")]
    public Dictionary<string, string>? Filter { get; set; }

    [JsonPropertyName("lookup")]
    public Dictionary<string, List<string>>? Lookup { get; set; }
}

public class SpellTypeOption
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
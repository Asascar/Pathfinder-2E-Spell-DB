using System.Text.Json.Serialization;

namespace PathfinderSpellDB.Models;

public class Spell
{
    [JsonPropertyName("nethysUrl")]
    public string? NethysUrl { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("traits")]
    public List<string> Traits { get; set; } = new();

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("legacy content")]
    public string? LegacyContent { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("traditions")]
    public List<string> Traditions { get; set; } = new();

    [JsonPropertyName("cast")]
    public string? Cast { get; set; }

    [JsonPropertyName("action")]
    public string? Action { get; set; }

    [JsonPropertyName("area")]
    public string? Area { get; set; }

    [JsonPropertyName("range")]
    public string? Range { get; set; }

    [JsonPropertyName("targets")]
    public string? Targets { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("saving throw")]
    public string? SavingThrow { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("components")]
    public List<string>? Components { get; set; }

    [JsonPropertyName("heightened")]
    public string? Heightened { get; set; }

    [JsonPropertyName("oldName")]
    public string? OldName { get; set; }
}
using System.Text.Json.Serialization;

namespace TwReport.Core.Models;

public class TaskwarriorTask
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("end")]
    public string? End { get; set; }

    [JsonPropertyName("entry")]
    public string Entry { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("project")]
    public string? Project { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("urgency")]
    public double Urgency { get; set; }

    public DateTime? GetEndDate()
    {
        if (string.IsNullOrEmpty(End))
            return null;

        if (DateTime.TryParseExact(End, "yyyyMMddTHHmmssZ",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal,
            out var date))
        {
            return date;
        }

        return null;
    }
}

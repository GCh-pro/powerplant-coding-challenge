
using System.Text.Json.Serialization;

namespace Models;

public class PowerPlantRequests
{
    [JsonPropertyName("load")]
    public int Load { get; init; }
    
    [JsonPropertyName("fuels")]
    public required Fuels Fuels { get; init; }

    [JsonPropertyName("powerplants")]
    public required List<PowerPlant> Powerplants { get; init; } = new();
}
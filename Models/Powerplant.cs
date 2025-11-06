
using System.Text.Json.Serialization;

namespace Models;

public class Fuels
{
    [JsonPropertyName("gas(euro/MWh)")]
    public double Gas { get; set; }

    [JsonPropertyName("kerosine(euro/MWh)")]
    public double Kerosine { get; set; }

    [JsonPropertyName("co2(euro/ton)")]
    public double Co2 { get; set; }

    [JsonPropertyName("wind(%)")]
    public double Wind { get; set; }
}


public class PowerPlant
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("efficiency")]
    public double Efficiency { get; set; }
    
    [JsonPropertyName("pmin")]
    public int Pmin { get; set; }
    
    [JsonPropertyName("pmax")]
    public int Pmax { get; set; }
}

public class PowerPlantRequest
{
    [JsonPropertyName("load")]
    public int Load { get; set; }
    
    [JsonPropertyName("fuels")]
    public required Fuels Fuels { get; set; }
    
    [JsonPropertyName("powerplants")]
    public required List<PowerPlant> Powerplants { get; set; } = new();
}

public class ProductionPlanItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    
    [JsonPropertyName("p")]
    public double P { get; set; }
}


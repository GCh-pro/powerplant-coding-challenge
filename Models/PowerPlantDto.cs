namespace Models; 

public class PowerPlantDto
{
    public int Load { get; init; }
    public double GasPrice { get; init; }
    public double KerosinePrice { get; init; }
    public double Co2Price { get; init; }
    public double WindPercentage { get; init; }
    public List<PowerPlant>? Powerplants { get; init; }
}
using Models;

namespace Services;

public class ProductionPlanner : IProductionPlanner
{
    public List<ProductionPlanItem> ComputePlan(PowerPlantRequest request)
    {
        double remainingLoad = request.Load;
        var result = new List<ProductionPlanItem>();

        // Calculate cost and available power for each plant, then sort by cost
        var plants = request.Powerplants
            .Select(p => new
            {
                Plant = p,
                Cost = CalculateCost(p, request.Fuels),
                Available = CalculateAvailablePower(p, request.Fuels)
            })
            .OrderBy(x => x.Cost)
            .ToList();

        // Allocate power to each plant in merit order (cheapest first)
        foreach (var p in plants)
        {
            if (remainingLoad <= 0)
            {
                // No load left, but we must include all plants with 0 power
                result.Add(new ProductionPlanItem { Name = p.Plant.Name, P = 0 });
                continue;
            }

            // Calculate how much this plant should produce
            double produced = CalculateProduction(p.Plant, p.Available, remainingLoad);

            result.Add(new ProductionPlanItem
            {
                Name = p.Plant.Name,
                P = Math.Round(produced, 1) // Round to 1 decimal for precision
            });

            remainingLoad -= produced;
        }

        // If we have a tiny remainder due to rounding, adjust the last non-zero plant
        if (Math.Abs(remainingLoad) > 0.01)
        {
            var lastNonZero = result.FindLast(x => x.P > 0);
            if (lastNonZero != null)
            {
                lastNonZero.P = Math.Round(lastNonZero.P - remainingLoad, 1);
            }
        }

        return result;
    }

    private double CalculateCost(PowerPlant plant, Fuels fuels)
    {
        return plant.Type switch
        {
            "gasfired" => (fuels.Gas / plant.Efficiency) + (0.3 * fuels.Co2),
            "turbojet" => (fuels.Kerosine / plant.Efficiency),
            "windturbine" => 0, // Wind is free!
            _ => double.MaxValue // Unknown plant types are most expensive
        };
    }

    private double CalculateAvailablePower(PowerPlant plant, Fuels fuels)
    {
        if (plant.Type == "windturbine")
        {
            // Wind turbine's Pmax is reduced by the wind percentage
            return plant.Pmax * (fuels.Wind / 100.0);
        }

        return plant.Pmax;
    }

    private double CalculateProduction(PowerPlant plant, double availablePower, double remainingLoad)
    {
        // For wind turbines, use whatever power is available up to the remaining load
        if (plant.Type == "windturbine")
        {
            return Math.Min(availablePower, remainingLoad);
        }

        // For other plants, respect Pmin constraint if possible
        if (remainingLoad >= plant.Pmin)
        {
            // We can satisfy Pmin, so use between Pmin and min(Pmax, remainingLoad)
            return Math.Min(availablePower, Math.Max(plant.Pmin, remainingLoad));
        }

        // Can't satisfy Pmin - if this is the last bit of load, generate it anyway
        return remainingLoad > 0 ? remainingLoad : 0;
    }
}

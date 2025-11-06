using System;
using System.Linq;
using System.Collections.Generic;
using Models;

namespace Services
{
    public class ProductionPlanner : IProductionPlanner
    {
        public List<ProductionPlanItem> ComputePlan(PowerPlantRequest request)
        {
            double remainingLoad = request.Load;
            var result = new List<ProductionPlanItem>();

            // Calculate if the cost and available power for each plant, then sort by ascending cost
            var plants = request.Powerplants
                .Select(p => new
                {
                    Plant = p,
                    Cost = CalculateCost(p, request.Fuels),
                    Available = CalculateAvailablePower(p, request.Fuels)
                })
                .OrderBy(x => x.Cost)
                .ToList();

            // Scan through plants in order of cost (merit order)
            foreach (var p in plants)
            {
                double production = 0;

                // For wind turbines, we produce up to the remaining load or the available power
                if (p.Plant.Type == "windturbine")
                {
                    production = Math.Min(p.Available, remainingLoad);
                }
                else
                {
                    // Check if we can produce at least Pmin
                    if (remainingLoad >= p.Plant.Pmin)
                    {
                        // Produce the maximum we can provide or what remains to be produced
                        production = Math.Min(p.Available, remainingLoad);
                    }
                    else
                    {
                        // If producing this plant would be below Pmin, we don't use it
                        production = 0;
                    }
                }

                result.Add(new ProductionPlanItem
                {
                    Name = p.Plant.Name,
                    P = Math.Round(production, 1)
                });

                remainingLoad -= production;

                // If the remaining load is complete, we can stop
                if (remainingLoad <= 0)
                    break;
            }

            // Fill remaining plants with 0 if they were not used
            foreach (var p in plants)
            {
                if (!result.Any(r => r.Name == p.Plant.Name))
                {
                    result.Add(new ProductionPlanItem { Name = p.Plant.Name, P = 0 });
                }
            }

            return result;
        }

        private double CalculateCost(PowerPlant plant, Fuels fuels)
        {
            return plant.Type switch
            {
                "gasfired" => (fuels.Gas / plant.Efficiency) + (0.244 * fuels.Co2),
                "turbojet" => (fuels.Kerosine / plant.Efficiency) + (0.267 * fuels.Co2),
                "windturbine" => 0,
                _ => double.MaxValue
            };
        }

        private double CalculateAvailablePower(PowerPlant plant, Fuels fuels)
        {
            if (plant.Type == "windturbine")
            {
                return plant.Pmax * (fuels.Wind / 100.0);
            }
            return plant.Pmax;
        }
    }
}

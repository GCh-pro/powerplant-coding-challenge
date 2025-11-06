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

            // Calculer le coût et la puissance disponible pour chaque centrale, puis trier par coût croissant
            var plants = request.Powerplants
                .Select(p => new
                {
                    Plant = p,
                    Cost = CalculateCost(p, request.Fuels),
                    Available = CalculateAvailablePower(p, request.Fuels)
                })
                .OrderBy(x => x.Cost)
                .ToList();

            // Parcourir les centrales dans l'ordre de coût (merit order)
            foreach (var p in plants)
            {
                double production = 0;

                // Pour les éoliennes, on produit jusqu'à la charge restante ou la puissance disponible
                if (p.Plant.Type == "windturbine")
                {
                    production = Math.Min(p.Available, remainingLoad);
                }
                else
                {
                    // Vérifier si on peut produire au moins Pmin
                    if (remainingLoad >= p.Plant.Pmin)
                    {
                        // On produit au maximum de ce qu'on peut fournir ou ce qui reste à produire
                        production = Math.Min(p.Available, remainingLoad);
                    }
                    else
                    {
                        // Si produire cette centrale serait en dessous de Pmin, on ne l'utilise pas
                        production = 0;
                    }
                }

                result.Add(new ProductionPlanItem
                {
                    Name = p.Plant.Name,
                    P = Math.Round(production, 1)
                });

                remainingLoad -= production;

                // Si la charge restante est complétée, on peut arrêter
                if (remainingLoad <= 0)
                    break;
            }

            // Remplir les centrales restantes avec 0 si elles n'ont pas été utilisées
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

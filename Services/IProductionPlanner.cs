using Models;
using System.Collections.Generic;

namespace Services
{
    public interface IProductionPlanner
    {
        List<ProductionPlanItem> ComputePlan(PowerPlantRequest request);
    }
}

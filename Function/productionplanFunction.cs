using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Models;
using Services; 
using Mapper;

namespace ProductionPlanFunction.Functions
{
    [ApiController]
    [Route("productionplan")] // use relative route (no leading slash) to avoid routing edge-cases
    public class PowerPlantController : ControllerBase
    {
    private readonly IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>> _mapper;
    private readonly IProductionPlanner _planner;

        // Inject the production planner via its interface so it can be mocked/tested
        public PowerPlantController(IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>> mapper, IProductionPlanner planner)
        {
            _mapper = mapper;
            _planner = planner;
        }

        [HttpPost]
        public IActionResult Post([FromBody] JsonElement data)
        {

                var request = JsonSerializer.Deserialize<PowerPlantRequest>(data.GetRawText());

                if (request == null)
                    return BadRequest("Invalid request body");

                var result = _mapper.Map(request);
                if (result.IsError)
                {
                    var errors = string.Join("; ", result.Errors);
                    return BadRequest(errors);
                }

            
                var productionPlan = _planner.ComputePlan(request);

          
                return Ok(productionPlan);
            }
    }
}

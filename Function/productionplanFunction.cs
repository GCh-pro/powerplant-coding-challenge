using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Models;
using Services; // <-- ton service ProductionPlanner
using Mapper;

namespace productionplanFunction.Functions
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
            try
            {
                // 🔹 1️⃣ Désérialisation du JSON reçu
                var request = JsonSerializer.Deserialize<PowerPlantRequest>(data.GetRawText());

                if (request == null)
                    return BadRequest("Invalid request body");

                // 🔹 2️⃣ Validation via ton mapper si tu veux garder cette logique
                var result = _mapper.Map(request);
                if (result.IsError)
                {
                    var errors = string.Join("; ", result.Errors);
                    return BadRequest(errors);
                }

                // 🔹 3️⃣ Calcul du plan de production
                var productionPlan = _planner.ComputePlan(request);

                // 🔹 4️⃣ Retourne le JSON attendu
                return Ok(productionPlan);
            }
            catch (JsonException ex)
            {
                return BadRequest($"JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}

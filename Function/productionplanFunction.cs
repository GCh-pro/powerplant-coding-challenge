using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Models;
using Mapper;

namespace productionplanFunction.Functions
{
    [ApiController]
    [Route("/powerplant")]
    public class PowerPlantController : ControllerBase
    {
        private readonly IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>> _mapper;

        public PowerPlantController(IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>> mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post([FromBody] JsonElement data)
        {
            try
            {
                var request = JsonSerializer.Deserialize<PowerPlantRequest>(data.GetRawText());

                if (request == null)
                    return BadRequest("Invalid request body");

                var result = _mapper.Map(request);

                if (result.IsError)
                {
                    // Renvoie toutes les erreurs comme texte (ou tu peux en choisir une seule)
                    var errors = string.Join("; ", result.Errors);
                    return BadRequest(errors);
                }

                return Ok(result.Result); // Objet mappé
            }
            catch (JsonException ex)
            {
                return BadRequest($"JSON parsing error: {ex.Message}");
            }
        }
    }
}

using Models;
using System.Collections.Generic;

namespace Mapper
{

    public class PowerPlantRequestMapper : IMapper<PowerPlantRequest, ErrorOr<PowerPlantDto>>
    {
        public ErrorOr<PowerPlantDto> Map(PowerPlantRequest input)
        {
            var errors = new List<string>();

            if (input == null)
                errors.Add("Request is null");

            if (input.Fuels == null)
                errors.Add("Fuels section is missing");

            if (input.Powerplants == null)
                errors.Add("Powerplants section is missing");

            if (errors.Count > 0)
                return Mapper.ErrorOr<PowerPlantDto>.Error(errors.ToArray());

            // Mapping securized
            return ErrorOr<PowerPlantDto>.Ok(new PowerPlantDto
            {
                Load = input.Load,
                GasPrice = input.Fuels?.Gas ?? 0,
                KerosinePrice = input.Fuels?.Kerosine ?? 0,
                Co2Price = input.Fuels?.Co2 ?? 0,
                WindPercentage = input.Fuels?.Wind ?? 0,
                Powerplants = input.Powerplants
            });
        }
    }
}

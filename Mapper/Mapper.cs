using Models;
using System.Collections.Generic;

namespace Mapper
{
    // Classe générique pour gérer le résultat ou les erreurs
    public class ErrorOr<T>
    {
        public T? Result { get; }
        public List<string> Errors { get; }
        public bool IsError => Errors.Count > 0;

        private ErrorOr(T result)
        {
            Result = result;
            Errors = new List<string>();
        }

        private ErrorOr(List<string> errors)
        {
            Result = default;
            Errors = errors;
        }

        public static ErrorOr<T> Ok(T result) => new ErrorOr<T>(result);
        public static ErrorOr<T> Error(params string[] errors) => new ErrorOr<T>(new List<string>(errors));
    }

    public class PowerPlantDto
    {
        public int Load { get; set; }
        public double GasPrice { get; set; }
        public double KerosinePrice { get; set; }
        public double Co2Price { get; set; }
        public double WindPercentage { get; set; }
        public List<PowerPlant>? Powerplants { get; set; }
    }

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
                return ErrorOr<PowerPlantDto>.Error(errors.ToArray());

            // Mapping sécurisé
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

namespace Trucks.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var despatchers = new List<Despatcher>();

            var despatchersDtos = XmlConverter.Deserializer<ImportDespatchersDto>(xmlString, "Despatchers");

            foreach (var despatcherDto in despatchersDtos)
            {
                if (!IsValid(despatcherDto) || string.IsNullOrEmpty(despatcherDto.Position))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }                

                var despatcher = new Despatcher()
                {
                    Name = despatcherDto.Name,
                    Position = despatcherDto.Position
                };

                foreach (var truckDto in despatcherDto.Trucks)
                {
                    if (!IsValid(truckDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var truck = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        TankCapacity = truckDto.TankCapacity,
                        CargoCapacity = truckDto.CargoCapacity,
                        CategoryType = Enum.Parse<CategoryType>(truckDto.CategoryType),
                        MakeType = Enum.Parse<MakeType>(truckDto.MakeType)
                    };

                    despatcher.Trucks.Add(truck);
                }
                despatchers.Add(despatcher);
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count));
            }

            context.AddRange(despatchers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var clientsDtos = JsonConvert.DeserializeObject<IEnumerable<ImportClientsDto>>(jsonString);

            var validClients = new List<Client>();

            var allIds = context.Trucks.Select(x => x.Id).ToHashSet();

            foreach (var clientsDto in clientsDtos)
            {
                var validTrucks = new List<ClientTruck>();

                if (!IsValid(clientsDto) || clientsDto.Type == "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var client = new Client()
                {
                    Name = clientsDto.Name,
                    Nationality = clientsDto.Nationality,
                    Type = clientsDto.Type,
                    ClientsTrucks = validTrucks
                };

                foreach (var id in clientsDto.Trucks.Distinct())
                {
                    if (!allIds.Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    validTrucks.Add(new ClientTruck
                    {
                        TruckId = id
                    });
                }

                validClients.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, validTrucks.Count));
            }
            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}

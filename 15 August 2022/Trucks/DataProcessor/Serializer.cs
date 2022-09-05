namespace Trucks.DataProcessor
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Trucks.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchers = context.Despatchers.Where(x => x.Trucks.Count > 0).ToArray().Select(x => new ExportDespatchersDto
            {
                TrucksCount = x.Trucks.Count,
                DespatcherName = x.Name,
                Trucks = x.Trucks.Select(t => new ExportTrucksDto
                {
                    RegistrationNumber = t.RegistrationNumber,
                    Make = t.MakeType.ToString()
                }).OrderBy(t => t.RegistrationNumber).ToArray()
            }).OrderByDescending(x => x.TrucksCount).ThenBy(x => x.DespatcherName).ToArray();
            
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                var xmlSerializer = new XmlSerializer(typeof(ExportDespatchersDto[]), new XmlRootAttribute("Despatchers"));

                xmlSerializer.Serialize(writer, despatchers, namespaces);
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity) 
        {
            var clients = context.Clients.Where(x => x.ClientsTrucks.Any(t => t.Truck.TankCapacity >= capacity)).ToArray().Select(x => new
            {
                Name = x.Name,
                Trucks = x.ClientsTrucks.Where(t => t.Truck.TankCapacity >= capacity).Select(x => new
                {
                    TruckRegistrationNumber = x.Truck.RegistrationNumber,
                    VinNumber = x.Truck.VinNumber,
                    TankCapacity = x.Truck.TankCapacity,
                    CargoCapacity = x.Truck.CargoCapacity,
                    CategoryType = x.Truck.CategoryType.ToString(),
                    MakeType = x.Truck.MakeType.ToString()
                }).OrderBy(x => x.MakeType).ThenByDescending(x => x.CargoCapacity).ToList()
            }).OrderByDescending(x => x.Trucks.Count).ThenBy(x => x.Name).Take(10).ToArray();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }
    }
}

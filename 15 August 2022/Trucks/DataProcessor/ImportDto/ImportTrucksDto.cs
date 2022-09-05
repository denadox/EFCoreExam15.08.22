using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTrucksDto
    {
        [Required]
        [RegularExpression("[A-Z]{2}[0-9]{4}[A-Z]{2}")]
        [StringLength(8)]
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [Required]
        [StringLength(17)]
        [XmlElement("VinNumber")]
        public string VinNumber { get; set; }
        
        [XmlElement("TankCapacity")]
        [Range(950, 1420)]
        public int TankCapacity { get; set; }
        
        [XmlElement("CargoCapacity")]
        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        [EnumDataType(typeof(CategoryType))]
        [XmlElement("CategoryType")]
        [Required]
        public string CategoryType { get; set; }

        [EnumDataType(typeof(MakeType))]
        [XmlElement("MakeType")]
        [Required]
        public string MakeType { get; set; }
    }
}


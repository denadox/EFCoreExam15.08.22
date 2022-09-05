using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {

        public Truck()
        {
            this.ClientsTrucks = new HashSet<ClientTruck>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(8)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MaxLength(17)]
        public string VinNumber { get; set; }

        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        public CategoryType CategoryType { get; set; }

        public MakeType MakeType { get; set; }

        public int DespatcherId { get; set; }
        public Despatcher Despatcher { get; set; }

        public ICollection<ClientTruck> ClientsTrucks { get; set; }
    }
}

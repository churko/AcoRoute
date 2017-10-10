using System;
using System.ComponentModel.DataAnnotations;

namespace AcoRoute.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        public string Plate { get; set; }
        public string VehicleType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }
        public DateTime LastServiceDate { get; set; }
    }
}
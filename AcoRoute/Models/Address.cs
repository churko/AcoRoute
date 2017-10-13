using System.ComponentModel.DataAnnotations;

namespace AcoRoute.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        public string StreetName { get; set; }
        public int AddressNumber { get; set; }
        public string FloorNumber { get; set; }
        public string FlatNumber { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
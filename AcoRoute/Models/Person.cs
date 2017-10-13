using System.ComponentModel.DataAnnotations;

namespace AcoRoute.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string DocumentType { get; set; }
        public int DocumentNumber { get; set; }
        public string StreetName { get; set; }
        public int AddressNumber { get; set; }
        public string FloorNumber { get; set; }
        public string FlatNumber { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int CellphoneNumber { get; set; }
        public int WorkphoneNumber { get; set; }
    }
}
using System;

namespace Kodhier.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string VIN { get; set; }
        public string Brand { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}

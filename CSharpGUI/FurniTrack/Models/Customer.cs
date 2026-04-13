using System;

namespace FurniTrack.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public double TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

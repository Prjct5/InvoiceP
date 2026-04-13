using System;

namespace FurniTrack.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = "";
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

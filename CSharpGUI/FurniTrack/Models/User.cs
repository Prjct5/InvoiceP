using System;

namespace FurniTrack.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "cashier";
        public int IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

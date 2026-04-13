using System;

namespace FurniTrack.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; } = "";
        public string EntityType { get; set; } = "";
        public int? EntityId { get; set; }
        public string? Summary { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}

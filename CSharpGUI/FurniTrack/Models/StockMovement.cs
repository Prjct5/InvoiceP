using System;

namespace FurniTrack.Models
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public string MovementType { get; set; } = "";
        public int Quantity { get; set; }
        public int StockAfter { get; set; }
        public string? Reason { get; set; }
        public string? ReferenceDoc { get; set; }
        public DateTime MovedAt { get; set; }
    }
}

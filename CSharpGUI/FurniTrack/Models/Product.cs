using System;

namespace FurniTrack.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public string Name { get; set; } = "";
        public string Sku { get; set; } = "";
        public string? Description { get; set; }
        public double UnitPrice { get; set; }
        public double CostPrice { get; set; }
        public int StockQty { get; set; }
        public int MinStock { get; set; }
        public string Unit { get; set; } = "piece";
        public string? ImagePath { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

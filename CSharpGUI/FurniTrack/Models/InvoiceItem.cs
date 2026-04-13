using System;

namespace FurniTrack.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountPct { get; set; }
        public double LineTotal { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductSku { get; set; } = "";
    }
}

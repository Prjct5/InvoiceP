using System;

namespace FurniTrack.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public string Status { get; set; } = "draft";
        public double Subtotal { get; set; }
        public double DiscountAmount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public double AmountPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

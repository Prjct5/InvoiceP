using System;

namespace FurniTrack.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public double AmountPaid { get; set; }
        public string Method { get; set; } = "cash";
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public DateTime PaidAt { get; set; }
    }
}

using System;

namespace FurniTrack.Models
{
    public class StoreSettings
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string? LogoPath { get; set; }
        public string? WhatsappNumber { get; set; }
        public string? PrinterName { get; set; }
        public string CurrencySymbol { get; set; } = "EGP";
        public double TaxRate { get; set; }
        public string InvoicePrefix { get; set; } = "INV";
        public int InvoiceNextNum { get; set; }
        public string? GithubRepo { get; set; }
        public string? GithubToken { get; set; }
        public string? GithubBranch { get; set; }
        public int AutoBackup { get; set; }
        public string ThemeColor { get; set; } = "#1ABC9C";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

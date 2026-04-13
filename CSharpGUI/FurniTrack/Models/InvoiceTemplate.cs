using System;

namespace FurniTrack.Models
{
    public class InvoiceTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int IsDefault { get; set; }
        public int ShowLogo { get; set; }
        public string LogoPosition { get; set; } = "left";
        public int LogoWidthPx { get; set; }
        public string HeaderBgColor { get; set; } = "#2C3E50";
        public string HeaderTextColor { get; set; } = "#FFFFFF";
        public string AccentColor { get; set; } = "#1ABC9C";
        public string TableHeaderColor { get; set; } = "#ECF0F1";
        public string RowAltColor { get; set; } = "#F9F9F9";
        public string FontFamily { get; set; } = "Arial";
        public int FontSizeBody { get; set; }
        public int FontSizeHeader { get; set; }
        public int ShowSku { get; set; }
        public int ShowUnit { get; set; }
        public int ShowDiscountCol { get; set; }
        public int ShowTaxLine { get; set; }
        public int ShowPaymentMethod { get; set; }
        public int ShowSignatureBox { get; set; }
        public int ShowQrCode { get; set; }
        public string? HeaderTagline { get; set; }
        public string? FooterText { get; set; }
        public string? TermsText { get; set; }
        public string PaperSize { get; set; } = "A4";
        public string Orientation { get; set; } = "portrait";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

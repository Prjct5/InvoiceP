using System;
using System.Data.SQLite;
using FurniTrack.Models;

namespace FurniTrack.Services
{
    public class SettingsService
    {
        private readonly Data.DatabaseManager _db;

        public SettingsService(Data.DatabaseManager db)
        {
            _db = db;
        }

        public bool IsFirstRun()
        {
            var settings = Load();
            return string.IsNullOrWhiteSpace(settings.StoreName) || settings.StoreName == "My Furniture Store";
        }

        public StoreSettings Load()
        {
            using var conn = _db.GetConnection();
            using var cmd = new SQLiteCommand("SELECT * FROM store_settings WHERE id = 1", conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new StoreSettings
                {
                    Id = Convert.ToInt32(reader["id"]),
                    StoreName = reader["store_name"]?.ToString() ?? "",
                    OwnerName = reader["owner_name"]?.ToString() ?? "",
                    Phone = reader["phone"]?.ToString() ?? "",
                    Address = reader["address"]?.ToString() ?? "",
                    LogoPath = reader["logo_path"]?.ToString(),
                    WhatsappNumber = reader["whatsapp_number"]?.ToString(),
                    PrinterName = reader["printer_name"]?.ToString(),
                    CurrencySymbol = reader["currency_symbol"]?.ToString() ?? "EGP",
                    TaxRate = Convert.ToDouble(reader["tax_rate"]),
                    InvoicePrefix = reader["invoice_prefix"]?.ToString() ?? "INV",
                    InvoiceNextNum = Convert.ToInt32(reader["invoice_next_num"]),
                    GithubRepo = reader["github_repo"]?.ToString(),
                    GithubToken = reader["github_token"]?.ToString(),
                    GithubBranch = reader["github_branch"]?.ToString(),
                    AutoBackup = Convert.ToInt32(reader["auto_backup"]),
                    ThemeColor = reader["theme_color"]?.ToString() ?? "#1ABC9C"
                };
            }
            return new StoreSettings();
        }

        public void Save(StoreSettings s)
        {
            using var conn = _db.GetConnection();
            var cmd = new SQLiteCommand(@"
                UPDATE store_settings SET 
                store_name = @sn, owner_name = @on, phone = @ph, address = @addr,
                logo_path = @logo, whatsapp_number = @wa, printer_name = @prn,
                currency_symbol = @curr, tax_rate = @tax, invoice_prefix = @pfx,
                github_repo = @repo, github_token = @tok, auto_backup = @auto,
                theme_color = @th, updated_at = datetime('now')
                WHERE id = 1", conn);
            
            cmd.Parameters.AddWithValue("@sn", s.StoreName);
            cmd.Parameters.AddWithValue("@on", s.OwnerName);
            cmd.Parameters.AddWithValue("@ph", s.Phone);
            cmd.Parameters.AddWithValue("@addr", s.Address);
            cmd.Parameters.AddWithValue("@logo", (object?)s.LogoPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@wa", (object?)s.WhatsappNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prn", (object?)s.PrinterName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@curr", s.CurrencySymbol);
            cmd.Parameters.AddWithValue("@tax", s.TaxRate);
            cmd.Parameters.AddWithValue("@pfx", s.InvoicePrefix);
            cmd.Parameters.AddWithValue("@repo", (object?)s.GithubRepo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tok", (object?)s.GithubToken ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@auto", s.AutoBackup);
            cmd.Parameters.AddWithValue("@th", s.ThemeColor);

            cmd.ExecuteNonQuery();
        }
    }
}

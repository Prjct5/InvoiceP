using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FurniTrack.Controls
{
    public class DashboardPanel : UserControl
    {
        private readonly Data.DatabaseManager _db;

        public DashboardPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            _db = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db"); // Dev connection
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblTitle = new Label 
            { 
                Text = "Dashboard", 
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            this.Controls.Add(lblTitle);

            // Fetch quick stats
            int totalInvoices = 0;
            double totalRevenue = 0;
            int lowStockItems = 0;

            using (var conn = _db.GetConnection())
            {
                using var cmd1 = new SQLiteCommand("SELECT COUNT(*), SUM(total_amount) FROM invoices WHERE status = 'paid'", conn);
                using var reader1 = cmd1.ExecuteReader();
                if(reader1.Read())
                {
                    totalInvoices = reader1.IsDBNull(0) ? 0 : reader1.GetInt32(0);
                    totalRevenue = reader1.IsDBNull(1) ? 0 : reader1.GetDouble(1);
                }

                using var cmd2 = new SQLiteCommand("SELECT COUNT(*) FROM products WHERE stock_qty <= min_stock AND stock_qty > 0", conn);
                lowStockItems = Convert.ToInt32(cmd2.ExecuteScalar());
            }

            var pnlRev = CreateStatCard("Total Revenue", $"EGP {totalRevenue:N2}", Color.FromArgb(26, 188, 156), new Point(30, 80));
            var pnlInv = CreateStatCard("Paid Invoices", totalInvoices.ToString(), Color.FromArgb(52, 152, 219), new Point(250, 80));
            var pnlStock = CreateStatCard("Low Stock Items", lowStockItems.ToString(), Color.FromArgb(231, 76, 60), new Point(470, 80));

            this.Controls.Add(pnlRev);
            this.Controls.Add(pnlInv);
            this.Controls.Add(pnlStock);
            
            // Recent invoices
            var lblRecent = new Label 
            { 
                Text = "Recent Invoices", 
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(30, 220),
                AutoSize = true,
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            this.Controls.Add(lblRecent);

            var grid = new DataGridView
            {
                Location = new Point(30, 260),
                Size = new Size(640, 300),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None
            };
            
            // Load some data
            grid.ColumnCount = 4;
            grid.Columns[0].Name = "Invoice #";
            grid.Columns[1].Name = "Date";
            grid.Columns[2].Name = "Total";
            grid.Columns[3].Name = "Status";

            using (var conn = _db.GetConnection())
            {
                using var cmd = new SQLiteCommand("SELECT invoice_number, invoice_date, total_amount, status FROM invoices ORDER BY id DESC LIMIT 10", conn);
                using var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    grid.Rows.Add(
                        reader["invoice_number"].ToString(),
                        Convert.ToDateTime(reader["invoice_date"]).ToString("yyyy-MM-dd"),
                        $"EGP {Convert.ToDouble(reader["total_amount"]):N2}",
                        reader["status"].ToString()?.ToUpper()
                    );
                }
            }

            this.Controls.Add(grid);
        }

        private Panel CreateStatCard(string title, string value, Color borderTop, Point location)
        {
            var pnl = new Panel
            {
                Size = new Size(200, 100),
                Location = location,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var topColor = new Panel { Height = 4, Dock = DockStyle.Top, BackColor = borderTop };
            var lblT = new Label { Text = title, Font = new Font("Segoe UI", 10, FontStyle.Regular), ForeColor = Color.Gray, Location = new Point(15, 15), AutoSize = true };
            var lblV = new Label { Text = value, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(15, 45), AutoSize = true };

            pnl.Controls.Add(lblV);
            pnl.Controls.Add(lblT);
            pnl.Controls.Add(topColor);
            return pnl;
        }
    }
}

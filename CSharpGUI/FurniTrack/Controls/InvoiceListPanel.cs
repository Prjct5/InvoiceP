using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FurniTrack.Controls
{
    public class InvoiceListPanel : UserControl
    {
        private readonly Data.DatabaseManager _db;
        
        public InvoiceListPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            _db = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db");
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            var lblTitle = new Label { Text = "Invoice History", Font = new Font("Segoe UI", 24, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
            this.Controls.Add(lblTitle);

            var grid = new DataGridView
            {
                Location = new Point(30, 100), Size = new Size(700, 480), BackgroundColor = Color.White,
                AllowUserToAddRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, BorderStyle = BorderStyle.None
            };
            this.Controls.Add(grid);
            
            var dt = new System.Data.DataTable();
            try {
                using (var conn = _db.GetConnection())
                {
                    using var cmd = new SQLiteCommand(@"
                        SELECT i.invoice_number as InvoiceNum, c.full_name as Customer, i.invoice_date as Date, i.total_amount as Total, i.status as Status 
                        FROM invoices i 
                        LEFT JOIN customers c ON i.customer_id = c.id 
                        ORDER BY i.id DESC", conn);
                    using var adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                grid.DataSource = dt;
            } catch {}
        }
    }
}

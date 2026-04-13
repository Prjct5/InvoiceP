using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FurniTrack.Controls
{
    public class CustomerPanel : UserControl
    {
        private readonly Data.DatabaseManager _db;
        
        public CustomerPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            _db = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db");
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            var lblTitle = new Label { Text = "Customers Database", Font = new Font("Segoe UI", 24, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
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
                    using var cmd = new SQLiteCommand("SELECT id as ID, full_name as Name, phone as Phone, whatsapp as Whatsapp, total_spent as Spent FROM customers ORDER BY full_name", conn);
                    using var adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                grid.DataSource = dt;
            } catch {}
        }
    }
}

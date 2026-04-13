using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FurniTrack.Controls
{
    public class TemplatePanel : UserControl
    {
        private readonly Data.DatabaseManager _db;
        
        public TemplatePanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            _db = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db");
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            var lblTitle = new Label { Text = "Invoice Templates", Font = new Font("Segoe UI", 24, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
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
                    using var cmd = new SQLiteCommand("SELECT id, name, is_default, header_bg_color, accent_color, paper_size FROM invoice_templates ORDER BY id", conn);
                    using var adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                grid.DataSource = dt;
            } catch {}
        }
    }
}

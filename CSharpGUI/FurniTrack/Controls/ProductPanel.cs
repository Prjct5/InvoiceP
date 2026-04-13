using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FurniTrack.Controls
{
    public class ProductPanel : UserControl
    {
        private readonly Data.DatabaseManager _db;
        private DataGridView grid;

        public ProductPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            _db = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblTitle = new Label 
            { 
                Text = "Products & Categories", 
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            this.Controls.Add(lblTitle);

            var btnAdd = new Button { Text = "Add Product", Location = new Point(30, 80), Width = 120, Height = 35, BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => MessageBox.Show("Add Product form goes here.");
            this.Controls.Add(btnAdd);

            grid = new DataGridView
            {
                Location = new Point(30, 130),
                Size = new Size(700, 450),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(grid);

            LoadData();
        }

        private void LoadData()
        {
            var dt = new System.Data.DataTable();
            using (var conn = _db.GetConnection())
            {
                using var cmd = new SQLiteCommand("SELECT p.id, p.sku, p.name, c.name as category, p.unit_price, p.stock_qty FROM products p LEFT JOIN categories c ON p.category_id = c.id", conn);
                using var adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);
            }
            grid.DataSource = dt;
        }
    }
}

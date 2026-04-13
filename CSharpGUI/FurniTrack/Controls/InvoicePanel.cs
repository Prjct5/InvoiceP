using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FurniTrack.Controls
{
    public class InvoicePanel : UserControl
    {
        private Data.DatabaseManager _db;
        private DataGridView gridItems;

        public InvoicePanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            _db = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblTitle = new Label { Text = "New Invoice", Font = new Font("Segoe UI", 24, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
            this.Controls.Add(lblTitle);

            var lblCust = new Label { Text = "Customer:", Location = new Point(30, 80), AutoSize = true };
            var cmbCust = new ComboBox { Location = new Point(120, 77), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            
            var lblTemp = new Label { Text = "Template:", Location = new Point(400, 80), AutoSize = true };
            var cmbTemp = new ComboBox { Location = new Point(480, 77), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            this.Controls.Add(lblCust); this.Controls.Add(cmbCust);
            this.Controls.Add(lblTemp); this.Controls.Add(cmbTemp);

            // Fetch Customers and Templates
            using (var conn = _db.GetConnection())
            {
                using var cmdC = new SQLiteCommand("SELECT id, full_name FROM customers ORDER BY full_name", conn);
                using var rC = cmdC.ExecuteReader();
                while(rC.Read()) cmbCust.Items.Add(new { Text = rC["full_name"].ToString(), Value = rC["id"] });

                using var cmdT = new SQLiteCommand("SELECT id, name FROM invoice_templates", conn);
                using var rT = cmdT.ExecuteReader();
                while(rT.Read()) cmbTemp.Items.Add(new { Text = rT["name"].ToString(), Value = rT["id"] });
            }

            if(cmbCust.Items.Count > 0) cmbCust.SelectedIndex = 0;
            if(cmbTemp.Items.Count > 0) cmbTemp.SelectedIndex = 0;

            var btnAddItem = new Button { Text = "Add Item", Location = new Point(30, 130), Width = 120, Height = 30, BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAddItem.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnAddItem);

            gridItems = new DataGridView
            {
                Location = new Point(30, 170),
                Size = new Size(700, 300),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            
            gridItems.Columns.Add("ProductID", "ID");
            gridItems.Columns.Add("ProductName", "Product");
            gridItems.Columns.Add("Qty", "Qty");
            gridItems.Columns.Add("UnitPrice", "Unit Price");
            gridItems.Columns.Add("Total", "Total");
            
            this.Controls.Add(gridItems);

            var pnlSummary = new Panel { Location = new Point(530, 480), Size = new Size(200, 150), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var lblSub = new Label { Text = "Subtotal: EGP 0.00", Location = new Point(10, 10), AutoSize = true };
            var lblTax = new Label { Text = "Tax (15%): EGP 0.00", Location = new Point(10, 40), AutoSize = true };
            var lblTot = new Label { Text = "TOTAL: EGP 0.00", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(10, 80), AutoSize = true };
            
            pnlSummary.Controls.Add(lblSub);
            pnlSummary.Controls.Add(lblTax);
            pnlSummary.Controls.Add(lblTot);
            this.Controls.Add(pnlSummary);

            var btnCheckout = new Button { Text = "Checkout & Print", Location = new Point(530, 640), Width = 200, Height = 40, BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Click += (s, e) => MessageBox.Show("Triggering Python Service (invoice_printer.py) via PythonBridge...");
            this.Controls.Add(btnCheckout);
        }
    }
}

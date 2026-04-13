using System;
using System.Drawing;
using System.Windows.Forms;
using FurniTrack.Services;

namespace FurniTrack.Controls
{
    public class ReportsPanel : UserControl
    {
        public ReportsPanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(249, 249, 249);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblTitle = new Label { Text = "Reports Engine", Font = new Font("Segoe UI", 24, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
            this.Controls.Add(lblTitle);

            var lblDesc = new Label { Text = "Generate PDF reports by triggering the internal python microservices.", Location = new Point(35, 75), AutoSize = true };
            this.Controls.Add(lblDesc);

            string[] reports = { "sales_summary", "stock_status", "customer_ranking", "product_ranking", "audit_trail" };
            int y = 120;
            
            foreach (var r in reports)
            {
                var btn = new Button
                {
                    Text = "Generate " + r.Replace("_", " ").ToUpper(),
                    Location = new Point(30, y),
                    Width = 250,
                    Height = 40,
                    BackColor = Color.FromArgb(44, 62, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                
                string reportType = r; // closure
                btn.Click += (s, e) => 
                {
                    btn.Text = "Generating...";
                    btn.Enabled = false;
                    
                    var res = PythonBridge.Call<object>("report_generator.py", new[] { 
                        "--db-path", "..\\..\\..\\..\\Database\\furnitrack.db", 
                        "--type", reportType,
                        "--output-dir", "C:\\temp"
                    });
                    
                    if (res.Success) MessageBox.Show("Report generated in C:\\temp");
                    else MessageBox.Show("Error: " + res.Error);
                    
                    btn.Text = "Generate " + reportType.Replace("_", " ").ToUpper();
                    btn.Enabled = true;
                };

                this.Controls.Add(btn);
                y += 50;
            }
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace FurniTrack.Controls
{
    public class SidebarPanel : Panel
    {
        public SidebarPanel()
        {
            this.Width = 220;
            this.Dock = DockStyle.Left;
            this.BackColor = Color.FromArgb(44, 62, 80);

            var lblLogo = new Label { Text = "FurniTrack", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblLogo);

            int y = 80;
            string[] tabs = { "Dashboard", "New Invoice", "All Invoices", "Products", "Customers", "Templates", "Reports", "Settings" };
            
            foreach (var t in tabs)
            {
                var btn = new Button
                {
                    Text = "  " + t,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = Color.White,
                    BackColor = t == "New Invoice" ? Color.FromArgb(26, 188, 156) : Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(0, y),
                    Width = 220,
                    Height = 40,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Cursor = Cursors.Hand;
                
                // Keep track of hover colors
                var originalColor = btn.BackColor;
                if(t != "New Invoice"){
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(52, 73, 94);
                    btn.MouseLeave += (s, e) => btn.BackColor = originalColor;
                }

                this.Controls.Add(btn);
                y += 40;
            }
        }
    }
}

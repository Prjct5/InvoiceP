using System;
using System.Drawing;
using System.Windows.Forms;

namespace FurniTrack.Controls
{
    public class CustomStatusStrip : StatusStrip
    {
        private ToolStripStatusLabel lblUser;
        private ToolStripStatusLabel lblDate;

        public CustomStatusStrip()
        {
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.ForeColor = Color.FromArgb(44, 62, 80);

            lblUser = new ToolStripStatusLabel { Text = $"User: {Services.AuthService.CurrentUser?.Username ?? "Guest"}" };
            lblDate = new ToolStripStatusLabel { Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Alignment = ToolStripItemAlignment.Right };

            var spring = new ToolStripStatusLabel { Spring = true }; // pushes items to the right

            this.Items.Add(lblUser);
            this.Items.Add(spring);
            this.Items.Add(lblDate);
            
            var timer = new System.Windows.Forms.Timer { Interval = 60000 };
            timer.Tick += (s, e) => lblDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            timer.Start();
        }
    }
}

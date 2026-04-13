using System;
using System.Drawing;
using System.Windows.Forms;

namespace FurniTrack.Forms
{
    public class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "FurniTrack - Main";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var sidebar = new Controls.SidebarPanel();
            var statusStrip = new Controls.CustomStatusStrip();
            
            var mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(249, 249, 249) // #F9F9F9
            };

            var dashboard = new Controls.DashboardPanel();
            mainContentPanel.Controls.Add(dashboard);

            this.Controls.Add(mainContentPanel); // Add fill first
            this.Controls.Add(sidebar); // Then left 
            this.Controls.Add(statusStrip); // Then bottom
            
            this.FormClosed += (s, e) => Application.Exit();
        }
    }
}

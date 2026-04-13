using System;
using System.Drawing;
using System.Windows.Forms;
using FurniTrack.Services;
using FurniTrack.Models;

namespace FurniTrack.Forms
{
    public class SplashForm : Form
    {
        private readonly SettingsService _settingsService;
        private TextBox txtStoreName, txtOwner, txtPhone, txtAddress;
        private TextBox txtAdminUser, txtAdminPass;
        private Button btnFinish;

        public SplashForm(SettingsService settingsService)
        {
            _settingsService = settingsService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "FurniTrack - First Run Setup";
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            this.Controls.Add(new Label { Text = "Welcome to FurniTrack Setup", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, y), AutoSize = true});
            
            y += 40;
            this.Controls.Add(new Label { Text = "Store Name:", Location = new Point(20, y), AutoSize = true });
            txtStoreName = new TextBox { Location = new Point(120, y), Width = 240 };
            this.Controls.Add(txtStoreName);

            y += 30;
            this.Controls.Add(new Label { Text = "Owner Name:", Location = new Point(20, y), AutoSize = true });
            txtOwner = new TextBox { Location = new Point(120, y), Width = 240 };
            this.Controls.Add(txtOwner);

            y += 30;
            this.Controls.Add(new Label { Text = "Phone:", Location = new Point(20, y), AutoSize = true });
            txtPhone = new TextBox { Location = new Point(120, y), Width = 240 };
            this.Controls.Add(txtPhone);

            y += 30;
            this.Controls.Add(new Label { Text = "Address:", Location = new Point(20, y), AutoSize = true });
            txtAddress = new TextBox { Location = new Point(120, y), Width = 240, Multiline = true, Height = 50 };
            this.Controls.Add(txtAddress);

            y += 70;
            this.Controls.Add(new Label { Text = "Create Admin User", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, y), AutoSize = true });

            y += 30;
            this.Controls.Add(new Label { Text = "Username:", Location = new Point(20, y), AutoSize = true });
            txtAdminUser = new TextBox { Location = new Point(120, y), Width = 240 };
            this.Controls.Add(txtAdminUser);

            y += 30;
            this.Controls.Add(new Label { Text = "Password:", Location = new Point(20, y), AutoSize = true });
            txtAdminPass = new TextBox { Location = new Point(120, y), Width = 240, UseSystemPasswordChar = true };
            this.Controls.Add(txtAdminPass);

            y += 50;
            btnFinish = new Button { Text = "Save and Finish", Location = new Point(230, y), Width = 130, Height = 30 };
            btnFinish.Click += BtnFinish_Click;
            this.Controls.Add(btnFinish);
        }

        private void BtnFinish_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text) || string.IsNullOrWhiteSpace(txtAdminUser.Text))
            {
                MessageBox.Show("Store Name and Admin Username are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var settings = _settingsService.Load();
            settings.StoreName = txtStoreName.Text;
            settings.OwnerName = txtOwner.Text;
            settings.Phone = txtPhone.Text;
            settings.Address = txtAddress.Text;
            _settingsService.Save(settings);

            // Since we need to insert the admin user and there's no UI for it yet, we just directly run the SQL
            using var conn = new Data.DatabaseManager("..\\..\\..\\..\\Database\\furnitrack.db").GetConnection(); // we can inject db string or pass it
            
            string hash = BCrypt.Net.BCrypt.HashPassword(txtAdminPass.Text);
            using var cmd = new System.Data.SQLite.SQLiteCommand("INSERT INTO users (username, password_hash, full_name, role) VALUES (@u, @p, @u, 'admin')", conn);
            cmd.Parameters.AddWithValue("@u", txtAdminUser.Text);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.ExecuteNonQuery();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

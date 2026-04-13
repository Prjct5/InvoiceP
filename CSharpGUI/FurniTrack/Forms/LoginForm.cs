using System;
using System.Drawing;
using System.Windows.Forms;
using FurniTrack.Services;

namespace FurniTrack.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Label lblError;
        private readonly AuthService _authService;

        public LoginForm(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "FurniTrack - Login";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            var lblTitle = new Label { Text = "Login to FurniTrack", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(50, 20), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
            
            var lblUser = new Label { Text = "Username", Location = new Point(50, 70), AutoSize = true };
            txtUser = new TextBox { Location = new Point(50, 90), Width = 230 };
            
            var lblPass = new Label { Text = "Password", Location = new Point(50, 120), AutoSize = true };
            txtPass = new TextBox { Location = new Point(50, 140), Width = 230, UseSystemPasswordChar = true };

            lblError = new Label { Text = "", Location = new Point(50, 170), ForeColor = Color.Red, AutoSize = true };

            btnLogin = new Button { Text = "Login", Location = new Point(50, 170), Width = 230, Height = 30, BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);
            this.Controls.Add(lblError);
            this.Controls.Add(btnLogin);
            
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";
            if (_authService.Login(txtUser.Text, txtPass.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblError.Text = "Invalid username or password.";
                lblError.Location = new Point(50, 205); // Move below button or adjust
            }
        }
    }
}

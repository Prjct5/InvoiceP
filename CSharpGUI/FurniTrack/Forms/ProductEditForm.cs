using System;
using System.Drawing;
using System.Windows.Forms;

namespace FurniTrack.Forms
{
    public class ProductEditForm : Form
    {
        public ProductEditForm()
        {
            this.Text = "Edit Product";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var lblName = new Label { Text = "Product Name:", Location = new Point(20, 20), AutoSize = true };
            var txtName = new TextBox { Location = new Point(120, 20), Width = 230 };

            var lblSku = new Label { Text = "SKU:", Location = new Point(20, 60), AutoSize = true };
            var txtSku = new TextBox { Location = new Point(120, 60), Width = 230 };

            var lblPrice = new Label { Text = "Unit Price:", Location = new Point(20, 100), AutoSize = true };
            var txtPrice = new TextBox { Location = new Point(120, 100), Width = 100 };

            var lblStock = new Label { Text = "Stock Qty:", Location = new Point(20, 140), AutoSize = true };
            var txtStock = new TextBox { Location = new Point(120, 140), Width = 100 };

            var btnSave = new Button { Text = "Save", Location = new Point(250, 200), Width = 100, Height = 30, BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => this.DialogResult = DialogResult.OK;

            this.Controls.Add(lblName); this.Controls.Add(txtName);
            this.Controls.Add(lblSku); this.Controls.Add(txtSku);
            this.Controls.Add(lblPrice); this.Controls.Add(txtPrice);
            this.Controls.Add(lblStock); this.Controls.Add(txtStock);
            this.Controls.Add(btnSave);
        }
    }
}

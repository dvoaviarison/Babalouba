using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Babalouba
{
    public class CreateAndRename : Form
    {
        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        public TextBox edtPlaylistName;
        private Panel panel1;
        private Label label2;

        public CreateAndRename(string defaultName = "")
        {
            this.InitializeComponent();
            this.edtPlaylistName.Text = defaultName;
        }

        private void OnPlaylistNameTextChanged(object sender, EventArgs e)
        {
            this.btnOK.Enabled = this.edtPlaylistName.Text.Replace(" ", string.Empty) != string.Empty;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CreateAndRename));
            this.btnCancel = new Button();
            this.btnOK = new Button();
            this.edtPlaylistName = new TextBox();
            this.label1 = new Label();
            this.panel1 = new Panel();
            this.label2 = new Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(210, 83);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new Point(129, 83);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.edtPlaylistName.AccessibleName = "edtPlaylistName";
            this.edtPlaylistName.Location = new Point(101, 52);
            this.edtPlaylistName.Name = "edtPlaylistName";
            this.edtPlaylistName.Size = new Size(184, 20);
            this.edtPlaylistName.TabIndex = 2;
            this.edtPlaylistName.TextChanged += new EventHandler(this.OnPlaylistNameTextChanged);
            this.label1.AutoSize = true;
            this.label1.ForeColor = Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, 192);
            this.label1.Location = new Point(13, 58);
            this.label1.Name = "label1";
            this.label1.Size = new Size(68, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Playlist name";
            this.panel1.BackColor = Color.FromArgb(64, 0, 0);
            this.panel1.Controls.Add((Control)this.label2);
            this.panel1.Location = new Point(0, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(297, 35);
            this.panel1.TabIndex = 4;
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Microsoft Sans Serif", 14f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.label2.ForeColor = Color.Yellow;
            this.label2.Location = new Point(50, 5);
            this.label2.Name = "label2";
            this.label2.Size = new Size(213, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "BABALOUBA PLAYLIST";
            this.label2.TextAlign = ContentAlignment.TopCenter;
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.Black;
            this.ClientSize = new Size(296, 111);
            this.Controls.Add((Control)this.panel1);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.edtPlaylistName);
            this.Controls.Add((Control)this.btnOK);
            this.Controls.Add((Control)this.btnCancel);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.Name = nameof(CreateAndRename);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Create/Rename PlayList";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

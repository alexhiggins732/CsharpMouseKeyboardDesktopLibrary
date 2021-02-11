using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalMacroRecorder
{

    public partial class RectangleSelect : Form
    {
        public RectangleSelect()
        {
            InitializeComponent();
            this.BackColor = Color.Lime;
            this.TransparencyKey = Color.Lime;
            this.groupBox1.MouseHover += GroupBox1_MouseHover;
            this.groupBox1.MouseHover += GroupBox1_MouseHover;
            this.Capture = true;

            SetSize();
            Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);
            TitleHeight = (screenRectangle.Top - this.Top);
            if (File.Exists("rect-location.json"))
            {
                var json = File.ReadAllText("rect-location.json");
                var selectSettings = JsonConvert.DeserializeObject<SelectSettings>(json);
                this.Location = selectSettings.Rect.Location;
                this.Width = selectSettings.Rect.Width;
                this.Height = selectSettings.Rect.Height;
                if (selectSettings.Rb540Checked) this.rb540p.Checked = true;
                if (selectSettings.Rb720Checked) this.rb720p.Checked = true;
                if (selectSettings.Rb1080hecked) this.rb1080p.Checked = true;
            }
            this.btnSet.Focus();
        }

        public int TitleHeight;

        void SaveSettings()
        {
            var settings = new SelectSettings();
            settings.Rect = this.Bounds;
            settings.Rb540Checked = this.rb540p.Checked;
            settings.Rb720Checked = this.rb720p.Checked;
            settings.Rb1080hecked = this.rb1080p.Checked;
            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText("rect-location.json", json);
        }

        void SetSize()
        {
            if (int.TryParse(txtWidth.Text, out int Width))
            {
                this.Width = Width;
            }
            if (int.TryParse(txtHeight.Text, out int Height))
            {
                this.Height = Height;
            }
        }

        private void GroupBox1_MouseHover(object sender, EventArgs e)
        {
            this.BackColor = Color.FromName("Control");
            this.TransparencyKey = Color.FromName("0");
        }

        public bool SelectedArea = false;
        private void btnSet_Click(object sender, EventArgs e)
        {
            SelectedArea = true;
            SaveSettings();
            this.Close();
        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {
            SetSize();
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {
            SetSize();
        }

        private void rb540p_CheckedChanged(object sender, EventArgs e)
        {
            if (rb540p.Checked)
            {
                this.txtHeight.Text = "540";
                this.txtWidth.Text = "940";
            }
        }

        private void rb720p_CheckedChanged(object sender, EventArgs e)
        {
            if (rb720p.Checked)
            {
                this.txtHeight.Text = "720";
                this.txtWidth.Text = "1280";
            }
        }

        private void rb1080p_CheckedChanged(object sender, EventArgs e)
        {
            if (rb720p.Checked)
            {
                this.txtHeight.Text = "720";
                this.txtWidth.Text = "1280";
            }
        }
    }
    public class SelectSettings
    {
        public Rectangle Rect;
        public bool Rb540Checked;
        public bool Rb720Checked;
        public bool Rb1080hecked;
    }
}

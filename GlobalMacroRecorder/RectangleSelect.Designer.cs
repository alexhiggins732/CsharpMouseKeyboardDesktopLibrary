
namespace GlobalMacroRecorder
{
    partial class RectangleSelect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSet = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb540p = new System.Windows.Forms.RadioButton();
            this.rb720p = new System.Windows.Forms.RadioButton();
            this.rb1080p = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(74, 17);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(100, 20);
            this.txtWidth.TabIndex = 0;
            this.txtWidth.Text = "940";
            this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Width:";
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(434, 15);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(75, 23);
            this.btnSet.TabIndex = 2;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Height:";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(228, 17);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(100, 20);
            this.txtHeight.TabIndex = 3;
            this.txtHeight.Text = "540";
            this.txtHeight.TextChanged += new System.EventHandler(this.txtHeight_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(334, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Maintain Ratio";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rb1080p);
            this.groupBox1.Controls.Add(this.rb720p);
            this.groupBox1.Controls.Add(this.rb540p);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.txtWidth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtHeight);
            this.groupBox1.Controls.Add(this.btnSet);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(745, 50);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // rb540p
            // 
            this.rb540p.AutoSize = true;
            this.rb540p.Location = new System.Drawing.Point(515, 18);
            this.rb540p.Name = "rb540p";
            this.rb540p.Size = new System.Drawing.Size(49, 17);
            this.rb540p.TabIndex = 6;
            this.rb540p.TabStop = true;
            this.rb540p.Text = "540p";
            this.rb540p.UseVisualStyleBackColor = true;
            this.rb540p.CheckedChanged += new System.EventHandler(this.rb540p_CheckedChanged);
            // 
            // rb720p
            // 
            this.rb720p.AutoSize = true;
            this.rb720p.Location = new System.Drawing.Point(570, 18);
            this.rb720p.Name = "rb720p";
            this.rb720p.Size = new System.Drawing.Size(49, 17);
            this.rb720p.TabIndex = 7;
            this.rb720p.TabStop = true;
            this.rb720p.Text = "720p";
            this.rb720p.UseVisualStyleBackColor = true;
            this.rb720p.CheckedChanged += new System.EventHandler(this.rb720p_CheckedChanged);
            // 
            // rb1080p
            // 
            this.rb1080p.AutoSize = true;
            this.rb1080p.Location = new System.Drawing.Point(625, 18);
            this.rb1080p.Name = "rb1080p";
            this.rb1080p.Size = new System.Drawing.Size(55, 17);
            this.rb1080p.TabIndex = 8;
            this.rb1080p.TabStop = true;
            this.rb1080p.Text = "1080p";
            this.rb1080p.UseVisualStyleBackColor = true;
            this.rb1080p.CheckedChanged += new System.EventHandler(this.rb1080p_CheckedChanged);
            // 
            // RectangleSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 610);
            this.Controls.Add(this.groupBox1);
            this.Name = "RectangleSelect";
            this.Text = "RectangleSelect";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb1080p;
        private System.Windows.Forms.RadioButton rb720p;
        private System.Windows.Forms.RadioButton rb540p;
    }
}
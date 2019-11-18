namespace GlobalMacroRecorder
{
    partial class MacroForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacroForm));
            this.recordStartButton = new System.Windows.Forms.Button();
            this.recordStopButton = new System.Windows.Forms.Button();
            this.playBackMacroButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PlayWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Hidewindow = new System.Windows.Forms.CheckBox();
            this.stoponselect = new System.Windows.Forms.CheckBox();
            this.HotkeyActivated = new System.Windows.Forms.CheckBox();
            this.ForeverLoop = new System.Windows.Forms.CheckBox();
            this.SaveMacro = new System.Windows.Forms.CheckBox();
            this.btnPlayJson = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // recordStartButton
            // 
            this.recordStartButton.Location = new System.Drawing.Point(216, 8);
            this.recordStartButton.Margin = new System.Windows.Forms.Padding(6);
            this.recordStartButton.Name = "recordStartButton";
            this.recordStartButton.Size = new System.Drawing.Size(192, 44);
            this.recordStartButton.TabIndex = 0;
            this.recordStartButton.Text = "Start";
            this.recordStartButton.UseVisualStyleBackColor = true;
            this.recordStartButton.Click += new System.EventHandler(this.recordStartButton_Click);
            // 
            // recordStopButton
            // 
            this.recordStopButton.Enabled = false;
            this.recordStopButton.Location = new System.Drawing.Point(414, 8);
            this.recordStopButton.Margin = new System.Windows.Forms.Padding(6);
            this.recordStopButton.Name = "recordStopButton";
            this.recordStopButton.Size = new System.Drawing.Size(204, 44);
            this.recordStopButton.TabIndex = 0;
            this.recordStopButton.Text = "Stop";
            this.recordStopButton.UseVisualStyleBackColor = true;
            this.recordStopButton.Click += new System.EventHandler(this.recordStopButton_Click);
            // 
            // playBackMacroButton
            // 
            this.playBackMacroButton.Location = new System.Drawing.Point(216, 63);
            this.playBackMacroButton.Margin = new System.Windows.Forms.Padding(6);
            this.playBackMacroButton.Name = "playBackMacroButton";
            this.playBackMacroButton.Size = new System.Drawing.Size(402, 48);
            this.playBackMacroButton.TabIndex = 1;
            this.playBackMacroButton.Text = "Play Back";
            this.playBackMacroButton.UseVisualStyleBackColor = true;
            this.playBackMacroButton.Click += new System.EventHandler(this.playBackMacroButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Record Macro:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 75);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Playback Macro:";
            // 
            // PlayWorker
            // 
            this.PlayWorker.WorkerReportsProgress = true;
            this.PlayWorker.WorkerSupportsCancellation = true;
            this.PlayWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.PlayWorker_DoWork);
            this.PlayWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.PlayWorker_ProgressChanged);
            this.PlayWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.PlayWorker_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(0, 240);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(652, 23);
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Visible = false;
            // 
            // Hidewindow
            // 
            this.Hidewindow.AutoSize = true;
            this.Hidewindow.Location = new System.Drawing.Point(24, 123);
            this.Hidewindow.Margin = new System.Windows.Forms.Padding(6);
            this.Hidewindow.Name = "Hidewindow";
            this.Hidewindow.Size = new System.Drawing.Size(170, 29);
            this.Hidewindow.TabIndex = 4;
            this.Hidewindow.Text = "Hide Window";
            this.Hidewindow.UseVisualStyleBackColor = true;
            // 
            // stoponselect
            // 
            this.stoponselect.AutoSize = true;
            this.stoponselect.Location = new System.Drawing.Point(216, 123);
            this.stoponselect.Margin = new System.Windows.Forms.Padding(6);
            this.stoponselect.Name = "stoponselect";
            this.stoponselect.Size = new System.Drawing.Size(184, 29);
            this.stoponselect.TabIndex = 5;
            this.stoponselect.Text = "Stop on Select";
            this.stoponselect.UseVisualStyleBackColor = true;
            // 
            // HotkeyActivated
            // 
            this.HotkeyActivated.AutoSize = true;
            this.HotkeyActivated.Location = new System.Drawing.Point(414, 123);
            this.HotkeyActivated.Margin = new System.Windows.Forms.Padding(6);
            this.HotkeyActivated.Name = "HotkeyActivated";
            this.HotkeyActivated.Size = new System.Drawing.Size(211, 29);
            this.HotkeyActivated.TabIndex = 7;
            this.HotkeyActivated.Text = "Hotkey Activation";
            this.HotkeyActivated.UseVisualStyleBackColor = true;
            this.HotkeyActivated.CheckedChanged += new System.EventHandler(this.HotkeyActivated_CheckedChanged);
            // 
            // ForeverLoop
            // 
            this.ForeverLoop.AutoSize = true;
            this.ForeverLoop.Location = new System.Drawing.Point(414, 167);
            this.ForeverLoop.Margin = new System.Windows.Forms.Padding(6);
            this.ForeverLoop.Name = "ForeverLoop";
            this.ForeverLoop.Size = new System.Drawing.Size(172, 29);
            this.ForeverLoop.TabIndex = 8;
            this.ForeverLoop.Text = "Forever Loop";
            this.ForeverLoop.UseVisualStyleBackColor = true;
            this.ForeverLoop.Visible = false;
            this.ForeverLoop.CheckedChanged += new System.EventHandler(this.ForeverLoop_CheckedChanged);
            // 
            // SaveMacro
            // 
            this.SaveMacro.AutoSize = true;
            this.SaveMacro.Checked = true;
            this.SaveMacro.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SaveMacro.Location = new System.Drawing.Point(24, 164);
            this.SaveMacro.Margin = new System.Windows.Forms.Padding(6);
            this.SaveMacro.Name = "SaveMacro";
            this.SaveMacro.Size = new System.Drawing.Size(93, 29);
            this.SaveMacro.TabIndex = 9;
            this.SaveMacro.Text = "Save";
            this.SaveMacro.UseVisualStyleBackColor = true;
            // 
            // btnPlayJson
            // 
            this.btnPlayJson.Location = new System.Drawing.Point(208, 155);
            this.btnPlayJson.Margin = new System.Windows.Forms.Padding(6);
            this.btnPlayJson.Name = "btnPlayJson";
            this.btnPlayJson.Size = new System.Drawing.Size(192, 44);
            this.btnPlayJson.TabIndex = 10;
            this.btnPlayJson.Text = "PlayJson";
            this.btnPlayJson.UseVisualStyleBackColor = true;
            this.btnPlayJson.Click += new System.EventHandler(this.btnPlayJson_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(640, 12);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(176, 55);
            this.btnRecord.TabIndex = 11;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // MacroForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 282);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btnPlayJson);
            this.Controls.Add(this.SaveMacro);
            this.Controls.Add(this.ForeverLoop);
            this.Controls.Add(this.HotkeyActivated);
            this.Controls.Add(this.stoponselect);
            this.Controls.Add(this.Hidewindow);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playBackMacroButton);
            this.Controls.Add(this.recordStopButton);
            this.Controls.Add(this.recordStartButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.Name = "MacroForm";
            this.Text = "Global Macro Recorder Example";
            this.Click += new System.EventHandler(this.MacroForm_Click);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MacroForm_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button recordStartButton;
        private System.Windows.Forms.Button recordStopButton;
        private System.Windows.Forms.Button playBackMacroButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker PlayWorker;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox Hidewindow;
        private System.Windows.Forms.CheckBox stoponselect;
        private System.Windows.Forms.CheckBox HotkeyActivated;
        private System.Windows.Forms.CheckBox ForeverLoop;
        private System.Windows.Forms.CheckBox SaveMacro;
        private System.Windows.Forms.Button btnPlayJson;
        private System.Windows.Forms.Button btnRecord;
    }
}


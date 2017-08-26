namespace LiveSplit.dotStart.PetThePup
{
    partial class AutosplitterSettings
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._allowTimerStart = new System.Windows.Forms.CheckBox();
            this._allowTimerReset = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._splitModeNever = new System.Windows.Forms.RadioButton();
            this._splitModeEvery = new System.Windows.Forms.RadioButton();
            this._splitModeEveryUnique = new System.Windows.Forms.RadioButton();
            this._splitModeAll = new System.Windows.Forms.RadioButton();
            this._allowRegistryAccess = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._allowTimerStart);
            this.flowLayoutPanel1.Controls.Add(this._allowTimerReset);
            this.flowLayoutPanel1.Controls.Add(this._allowRegistryAccess);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(222, 196);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _allowTimerStart
            // 
            this._allowTimerStart.AutoSize = true;
            this._allowTimerStart.Location = new System.Drawing.Point(8, 8);
            this._allowTimerStart.Name = "_allowTimerStart";
            this._allowTimerStart.Size = new System.Drawing.Size(188, 17);
            this._allowTimerStart.TabIndex = 0;
            this._allowTimerStart.Text = "Allow to Start Timer on New Game";
            this._allowTimerStart.UseVisualStyleBackColor = true;
            // 
            // _allowTimerReset
            // 
            this._allowTimerReset.AutoSize = true;
            this._allowTimerReset.Location = new System.Drawing.Point(8, 31);
            this._allowTimerReset.Name = "_allowTimerReset";
            this._allowTimerReset.Size = new System.Drawing.Size(194, 17);
            this._allowTimerReset.TabIndex = 1;
            this._allowTimerReset.Text = "Allow to Reset Timer on New Game";
            this._allowTimerReset.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.flowLayoutPanel2);
            this.groupBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.groupBox1.Location = new System.Drawing.Point(8, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(101, 111);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Split Mode";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this._splitModeNever);
            this.flowLayoutPanel2.Controls.Add(this._splitModeEvery);
            this.flowLayoutPanel2.Controls.Add(this._splitModeEveryUnique);
            this.flowLayoutPanel2.Controls.Add(this._splitModeAll);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(95, 92);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // _splitModeNever
            // 
            this._splitModeNever.AutoSize = true;
            this._splitModeNever.Location = new System.Drawing.Point(3, 3);
            this._splitModeNever.Name = "_splitModeNever";
            this._splitModeNever.Size = new System.Drawing.Size(51, 17);
            this._splitModeNever.TabIndex = 0;
            this._splitModeNever.TabStop = true;
            this._splitModeNever.Text = "None";
            this._splitModeNever.UseVisualStyleBackColor = true;
            this._splitModeNever.CheckedChanged += this.OnSplitModeChanged;
            // 
            // _splitModeEvery
            // 
            this._splitModeEvery.AutoSize = true;
            this._splitModeEvery.Location = new System.Drawing.Point(3, 26);
            this._splitModeEvery.Name = "_splitModeEvery";
            this._splitModeEvery.Size = new System.Drawing.Size(52, 17);
            this._splitModeEvery.TabIndex = 1;
            this._splitModeEvery.TabStop = true;
            this._splitModeEvery.Text = "Every";
            this._splitModeEvery.UseVisualStyleBackColor = true;
            this._splitModeEvery.CheckedChanged += this.OnSplitModeChanged;
            // 
            // _splitModeEveryUnique
            // 
            this._splitModeEveryUnique.AutoSize = true;
            this._splitModeEveryUnique.Location = new System.Drawing.Point(3, 49);
            this._splitModeEveryUnique.Name = "_splitModeEveryUnique";
            this._splitModeEveryUnique.Size = new System.Drawing.Size(89, 17);
            this._splitModeEveryUnique.TabIndex = 2;
            this._splitModeEveryUnique.TabStop = true;
            this._splitModeEveryUnique.Text = "Every Unique";
            this._splitModeEveryUnique.UseVisualStyleBackColor = true;
            this._splitModeEveryUnique.CheckedChanged += this.OnSplitModeChanged;
            // 
            // _splitModeAll
            // 
            this._splitModeAll.AutoSize = true;
            this._splitModeAll.Location = new System.Drawing.Point(3, 72);
            this._splitModeAll.Name = "_splitModeAll";
            this._splitModeAll.Size = new System.Drawing.Size(73, 17);
            this._splitModeAll.TabIndex = 3;
            this._splitModeAll.TabStop = true;
            this._splitModeAll.Text = "All Unique";
            this._splitModeAll.UseVisualStyleBackColor = true;
            this._splitModeAll.CheckedChanged += this.OnSplitModeChanged;
            // 
            // _allowRegistryAccess
            // 
            this._allowRegistryAccess.AutoSize = true;
            this._allowRegistryAccess.Location = new System.Drawing.Point(8, 54);
            this._allowRegistryAccess.Name = "_allowRegistryAccess";
            this._allowRegistryAccess.Size = new System.Drawing.Size(206, 17);
            this._allowRegistryAccess.TabIndex = 3;
            this._allowRegistryAccess.Text = "Allow write access to the Pup Gallery";
            this._allowRegistryAccess.UseVisualStyleBackColor = true;
            // 
            // PetThePupSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "PetThePupSettings";
            this.Size = new System.Drawing.Size(222, 196);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox _allowTimerStart;
        private System.Windows.Forms.CheckBox _allowTimerReset;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.RadioButton _splitModeNever;
        private System.Windows.Forms.RadioButton _splitModeEvery;
        private System.Windows.Forms.RadioButton _splitModeEveryUnique;
        private System.Windows.Forms.RadioButton _splitModeAll;
        private System.Windows.Forms.CheckBox _allowRegistryAccess;
    }
}

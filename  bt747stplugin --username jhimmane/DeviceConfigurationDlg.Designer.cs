namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    partial class DeviceConfigurationDlg
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
            this.btnOk = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnCancel = new ZoneFiveSoftware.Common.Visuals.Button();
            this.chkImportOnlyNew = new System.Windows.Forms.CheckBox();
            this.txtCOMPort = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.labelCOMPort = new System.Windows.Forms.Label();
            this.txtTrackChange = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.trackLabel = new System.Windows.Forms.Label();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.waitLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnOk.CenterImage = null;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.HyperlinkStyle = false;
            this.btnOk.ImageMargin = 2;
            this.btnOk.LeftImage = null;
            this.btnOk.Location = new System.Drawing.Point(113, 147);
            this.btnOk.Name = "btnOk";
            this.btnOk.PushStyle = true;
            this.btnOk.RightImage = null;
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnOk.TextLeftMargin = 2;
            this.btnOk.TextRightMargin = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnCancel.CenterImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.HyperlinkStyle = false;
            this.btnCancel.ImageMargin = 2;
            this.btnCancel.LeftImage = null;
            this.btnCancel.Location = new System.Drawing.Point(193, 147);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PushStyle = true;
            this.btnCancel.RightImage = null;
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnCancel.TextLeftMargin = 2;
            this.btnCancel.TextRightMargin = 2;
            this.btnCancel.UseWaitCursor = false;
            // 
            // chkImportOnlyNew
            // 
            this.chkImportOnlyNew.AutoSize = true;
            this.chkImportOnlyNew.Checked = true;
            this.chkImportOnlyNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImportOnlyNew.Location = new System.Drawing.Point(15, 12);
            this.chkImportOnlyNew.Name = "chkImportOnlyNew";
            this.chkImportOnlyNew.Size = new System.Drawing.Size(124, 17);
            this.chkImportOnlyNew.TabIndex = 0;
            this.chkImportOnlyNew.Text = "Import new data only";
            this.chkImportOnlyNew.UseVisualStyleBackColor = true;
            this.chkImportOnlyNew.UseWaitCursor = false;
            // 
            // txtCOMPort
            // 
            this.txtCOMPort.AcceptsReturn = false;
            this.txtCOMPort.AcceptsTab = false;
            this.txtCOMPort.BackColor = System.Drawing.Color.White;
            this.txtCOMPort.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.txtCOMPort.ButtonImage = null;
            this.txtCOMPort.Location = new System.Drawing.Point(129, 38);
            this.txtCOMPort.MaxLength = 32767;
            this.txtCOMPort.Multiline = false;
            this.txtCOMPort.Name = "txtCOMPort";
            this.txtCOMPort.ReadOnly = false;
            this.txtCOMPort.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.txtCOMPort.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.txtCOMPort.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtCOMPort.Size = new System.Drawing.Size(58, 19);
            this.txtCOMPort.TabIndex = 1;
            this.txtCOMPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtCOMPort.UseWaitCursor = false;
            // 
            // labelCOMPort
            // 
            this.labelCOMPort.Location = new System.Drawing.Point(12, 40);
            this.labelCOMPort.Name = "labelCOMPort";
            this.labelCOMPort.Size = new System.Drawing.Size(100, 19);
            this.labelCOMPort.TabIndex = 1;
            this.labelCOMPort.Text = "COM Port:";
            this.labelCOMPort.UseWaitCursor = false;
            // 
            // txtTrackChange
            // 
            this.txtTrackChange.AcceptsReturn = false;
            this.txtTrackChange.AcceptsTab = false;
            this.txtTrackChange.BackColor = System.Drawing.Color.White;
            this.txtTrackChange.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.txtTrackChange.ButtonImage = null;
            this.txtTrackChange.Location = new System.Drawing.Point(130, 63);
            this.txtTrackChange.MaxLength = 32767;
            this.txtTrackChange.Multiline = false;
            this.txtTrackChange.Name = "txtTrackChange";
            this.txtTrackChange.ReadOnly = false;
            this.txtTrackChange.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.txtTrackChange.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.txtTrackChange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTrackChange.Size = new System.Drawing.Size(58, 19);
            this.txtTrackChange.TabIndex = 2;
            this.txtTrackChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtTrackChange.UseWaitCursor = false;
            // 
            // trackLabel
            // 
            this.trackLabel.Location = new System.Drawing.Point(12, 63);
            this.trackLabel.Name = "trackLabel";
            this.trackLabel.Size = new System.Drawing.Size(112, 19);
            this.trackLabel.TabIndex = 6;
            this.trackLabel.Text = "Track change (min):";
            this.trackLabel.UseWaitCursor = false;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(130, 89);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 3;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.UseWaitCursor = false;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // waitLabel
            // 
            this.waitLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.waitLabel.Location = new System.Drawing.Point(51, 115);
            this.waitLabel.Name = "waitLabel";
            this.waitLabel.Size = new System.Drawing.Size(137, 19);
            this.waitLabel.TabIndex = 7;
            this.waitLabel.Text = "Clearing log, please wait...";
            this.waitLabel.Visible = false;
            // 
            // DeviceConfigurationDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 175);
            this.Controls.Add(this.waitLabel);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.trackLabel);
            this.Controls.Add(this.txtTrackChange);
            this.Controls.Add(this.txtCOMPort);
            this.Controls.Add(this.labelCOMPort);
            this.Controls.Add(this.chkImportOnlyNew);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceConfigurationDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BT747 Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZoneFiveSoftware.Common.Visuals.Button btnOk;
        private ZoneFiveSoftware.Common.Visuals.Button btnCancel;
        private System.Windows.Forms.CheckBox chkImportOnlyNew;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtCOMPort;
        private System.Windows.Forms.Label labelCOMPort;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtTrackChange;
        private System.Windows.Forms.Label trackLabel;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label waitLabel;
    }
}
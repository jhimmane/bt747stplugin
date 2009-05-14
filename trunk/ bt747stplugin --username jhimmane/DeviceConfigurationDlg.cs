using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Visuals;

namespace ZoneFiveSoftware.SportTracks.Device.BT747
{
    public partial class DeviceConfigurationDlg : Form
    {
        public DeviceConfigurationDlg()
        {
            InitializeComponent();

            Text = CommonResources.Text.Devices.ConfigurationDialog_Title;
            chkImportOnlyNew.Text = Properties.Resources.DeviceConfigurationDlg_chkImportOnlyNew_Text;
            labelCOMPort.Text = Properties.Resources.DeviceConfigurationDlg_portConfig_Text;            
            btnOk.Text = CommonResources.Text.ActionOk;
            btnCancel.Text = CommonResources.Text.ActionCancel;
            
            if (Plugin.Instance.Application != null)
            {
                ThemeChanged(Plugin.Instance.Application.VisualTheme);
            }

            txtCOMPort.Validated += new EventHandler(txtCOMPort_Validated);
            btnOk.Click += new EventHandler(btnOk_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        #region Public properties

        internal DeviceConfigurationInfo ConfigurationInfo
        {
            get
            {
                DeviceConfigurationInfo configInfo = DeviceConfigurationInfo.Parse(null);
                configInfo.ImportOnlyNew = chkImportOnlyNew.Checked;
                configInfo.debug = debugBox.Checked;
                configInfo.portNumber = int.Parse(txtCOMPort.Text);
                configInfo.trackChange = int.Parse(txtTrackChange.Text);
                return configInfo;
            }
            set
            {
                chkImportOnlyNew.Checked = value.ImportOnlyNew;
                debugBox.Checked = value.debug;
                txtCOMPort.Text = value.portNumber.ToString();
                txtTrackChange.Text = value.trackChange.ToString();
            }
        }
        #endregion

        #region Public methods
        public void ThemeChanged(ITheme visualTheme)
        {
            theme = visualTheme;
            labelCOMPort.ForeColor = visualTheme.ControlText;
            txtCOMPort.ThemeChanged(visualTheme);
            chkImportOnlyNew.ForeColor = visualTheme.ControlText;
            BackColor = visualTheme.Control;
        }
        #endregion

        #region Event handlers
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            MessageDialog.DrawButtonRowBackground(e.Graphics, ClientRectangle, theme);
        }

        void txtCOMPort_Validated(object sender, EventArgs e)
        {
            int value = (int)double.Parse(txtCOMPort.Text);
            try
            {
                if (txtCOMPort.Text.Trim().Length == 0)
                {
                    value = 0;
                }
                else
                {
                    value = int.Parse(txtCOMPort.Text);
                }
            }
            catch { }
            
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = btnOk.DialogResult;            
            Close();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        void btnClearLog_Click(object sender, EventArgs e)
        {   
            BT747Device device = new BT747Device(ConfigurationInfo);
            DialogResult dr = new DialogResult();

            dr = MessageDialog.Show("Clear log, are you sure?", "Clearing log", MessageBoxButtons.OKCancel);
    
            if (dr == DialogResult.OK)                
            {
                this.btnOk.Enabled = false;
                this.btnCancel.Enabled = false;
                this.btnClearLog.Visible = false;
                this.waitLabel.Visible = true;
                this.Cursor = Cursors.WaitCursor;
                
                try
                {
                    device.Open((int)double.Parse(txtCOMPort.Text));
                    device.emptyLog();
                }
                finally
                {
                    device.Close();
                }
            }
            //else MessageDialog.Show("Cancel");

            this.waitLabel.Visible = false;
            this.btnOk.Enabled = true;
            this.btnCancel.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region Private methods

        #endregion

        #region Private members
        private ITheme theme;
        #endregion




    }
}
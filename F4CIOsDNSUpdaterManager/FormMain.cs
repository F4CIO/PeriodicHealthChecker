using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;

using F4CIOsDNSUpdaterCommon;

namespace F4CIOsDNSUpdaterManager
{
    public partial class FormMain : Form
    {
        public static Thread workingThread;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {                
                wbResponse.DocumentText = Threads.DoJob(tbUrl.Text, MailAlertShouldBeSent.Never, this.cbMailServerIsOffice365.Checked, this.tbMailServerAddress.Text, (int)this.nudMailServerPort.Value, this.tbMailServerUsername.Text, this.tbMailServerPassword.Text, this.tbMailTo.Text, this.tbMailSubject.Text);
			}
            catch (Exception exception)
            {                
                MessageBox.Show(exception.Message);
                tbUrl.Focus();
            }
        }

		private void btnTestMail_Click(object sender, EventArgs e)
		{
			try
            {                
                wbResponse.DocumentText = Threads.DoJob(tbUrl.Text, MailAlertShouldBeSent.InAnyCase, this.cbMailServerIsOffice365.Checked, this.tbMailServerAddress.Text, (int)this.nudMailServerPort.Value, this.tbMailServerUsername.Text, this.tbMailServerPassword.Text, this.tbMailTo.Text, this.tbMailSubject.Text);
			}
            catch (Exception exception)
            {                
                MessageBox.Show(exception.Message);
                tbMailServerAddress.Focus();
            }
		}

		private void F4CIOsDNSUpdaterManager_Load(object sender, EventArgs e)
        {
            
            try
            {
                long intervalInMiliseconds = DataAccess.GetInterval();                
                decimal intervalInMinutes =  intervalInMiliseconds /1000/60;
                this.nudInterval.Value = intervalInMinutes;
                this.tbUrl.Text = DataAccess.GetUri();
	            this.nudRetryAfterXMinutes.Value = DataAccess.GetRetryAfterXMinutes();
	            this.cbMailServerIsOffice365.Checked = DataAccess.GetMailServerIsOffice365();
	            this.tbMailServerAddress.Text = DataAccess.GetMailServerAddress();
	            this.nudMailServerPort.Value = DataAccess.GetMailServerPort();
	            this.tbMailServerUsername.Text = DataAccess.GetMailServerUsername();
	            this.tbMailServerPassword.Text = DataAccess.GetMailServerPassword();
	            this.tbMailTo.Text = DataAccess.GetMailServerTo();
	            this.tbMailSubject.Text = DataAccess.GetMailSubject();

	            //For debuging common project uncoment next line
	            //FormMain.workingThread = Threads.StartWorkingThread();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveValues();
                
                this.HidePanel();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }

        private void SaveValues()
        {
            long intervalInMiliseconds = Convert.ToInt64(this.nudInterval.Value) * 1000 * 60;
            DataAccess.SaveSettings(intervalInMiliseconds, this.tbUrl.Text, (int) this.nudRetryAfterXMinutes.Value, this.cbMailServerIsOffice365.Checked, this.tbMailServerAddress.Text, (int)this.nudMailServerPort.Value, this.tbMailServerUsername.Text, this.tbMailServerPassword.Text, this.tbMailTo.Text, this.tbMailSubject.Text);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try{
            scMain.Start();
              }
            catch (Exception exception)
            {
                tbServiceStatus.Text = exception.Message;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try{
            scMain.Stop();
              }
            catch (Exception exception)
            {
                tbServiceStatus.Text = exception.Message;
            }
        }

        private string GetStatusString(System.ServiceProcess.ServiceControllerStatus status)
        {
            string retVal ="";
            if (status == System.ServiceProcess.ServiceControllerStatus.ContinuePending) retVal = System.ServiceProcess.ServiceControllerStatus.ContinuePending.ToString();
            if (status == System.ServiceProcess.ServiceControllerStatus.Paused) retVal = System.ServiceProcess.ServiceControllerStatus.Paused.ToString();
            if (status == System.ServiceProcess.ServiceControllerStatus.PausePending) retVal = System.ServiceProcess.ServiceControllerStatus.PausePending.ToString();
            if (status == System.ServiceProcess.ServiceControllerStatus.Running) retVal = System.ServiceProcess.ServiceControllerStatus.Running.ToString();
            if (status == System.ServiceProcess.ServiceControllerStatus.StartPending) retVal = System.ServiceProcess.ServiceControllerStatus.StartPending.ToString();
            if (status == System.ServiceProcess.ServiceControllerStatus.Stopped) retVal = System.ServiceProcess.ServiceControllerStatus.Stopped.ToString();
            if (status == System.ServiceProcess.ServiceControllerStatus.StopPending) retVal = System.ServiceProcess.ServiceControllerStatus.StopPending.ToString();
            return retVal;
        }

        private void tmrStatusRefresher_Tick(object sender, EventArgs e)
        {
            try
            {
                scMain.Refresh();
                string newStatusString = GetStatusString(scMain.Status);
                if (newStatusString != tbServiceStatus.Text)
                {
                    tbServiceStatus.Text = newStatusString;
                }
            }
            catch (Exception exception)
            {
                tbServiceStatus.Text = exception.Message;
            }
        }

        private void niF4CIOsDNSUpdaterManager_BalloonTipShown(object sender, EventArgs e)
        {
            //this.niMain.BalloonTipText = "Service: "+GetStatusString(scMain.Status);
        }

        private void niF4CIOsDNSUpdaterManager_DoubleClick(object sender, EventArgs e)
        {
            this.ShowPanel();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.HidePanel();
        }

        private void msMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void cmsMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

                
        }

        private void tsmiShow_Click(object sender, EventArgs e)
        {
            this.ShowPanel();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.SaveValues();

            try
            {
                if (FormMain.workingThread != null)
                {
                    FormMain.workingThread.Abort();
                }
            }
            catch (Exception exception)
            {
                DataAccess.WriteLog(DateTime.Now + " Error occured while aborting working thread. Error details:"+exception.Message );
            }

            this.Close();
        }

        private void F4CIOsDNSUpdaterManager_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        private void ShowPanel()
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.tmrStatusRefresher.Enabled = true;
            this.BringToFront();
            this.Focus();
        }

        private void HidePanel()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.tmrStatusRefresher.Enabled = false;
        }

		private void nudInterval_ValueChanged(object sender, EventArgs e)
		{

		}

		
	}
}

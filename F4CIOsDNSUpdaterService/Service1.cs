using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;

using F4CIOsDNSUpdaterCommon;

namespace F4CIOsDNSUpdaterService
{
    public partial class svcF4CIOsDNSUpdater : ServiceBase
    {
        Thread workingThread = null;

        public svcF4CIOsDNSUpdater()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.workingThread = Threads.StartWorkingThread();
                DataAccess.WriteLog(DateTime.Now+" Service started.");
            }
            catch (Exception exception)
            {
                DataAccess.WriteLog(DateTime.Now + " Errror occured while starting service: " + exception.Message);                
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (this.workingThread != null)
                {
                    this.workingThread.Abort();
                }
                DataAccess.WriteLog(DateTime.Now + " Service stopped.");
            }
            catch (Exception exception)
            {
                DataAccess.WriteLog(DateTime.Now + " Error occured while aborting working thread and stopping service. Error details:" + exception.Message);
            }            
        }
    }
}

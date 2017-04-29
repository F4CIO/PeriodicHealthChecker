using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace F4CIOsDNSUpdaterService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new svcF4CIOsDNSUpdater() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}

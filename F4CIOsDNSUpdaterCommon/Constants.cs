using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace F4CIOsDNSUpdaterCommon
{
	public class Constants
	{
		static string ProgramFolder
		{
			get { return CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath; }
		}
		const string IniFileName = "F4CIOsDNSUpdater.ini";
		const string LogFileName = "F4CIOsDNSUpdater.log";
		const string HtmFileName = "LastResponse.htm";

		public static string IniFilePath
		{
			get{
				return ProgramFolder+"\\"+IniFileName;
			}
		}

		public static string LogFilePath
		{
			get{
				return ProgramFolder + "\\" + LogFileName;
			}
		}

		public static string HtmFilePath
		{
			get
			{
				return ProgramFolder + "\\" + HtmFileName;
			}
		}
	}
}

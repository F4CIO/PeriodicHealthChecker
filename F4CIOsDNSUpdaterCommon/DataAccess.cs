using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace F4CIOsDNSUpdaterCommon
{
    public class DataAccess
    {
        private static Object iniLock = new Object();
        private static Object logLock = new Object();
        private static Object htmLock = new Object();

        public static void WriteLog(string logString)
        {
            lock (logLock)
            {
                try
                {

                    if (!File.Exists(Constants.LogFilePath))
                    {
                        using (StreamWriter sw = File.CreateText(Constants.LogFilePath))
                        {
                            sw.Close();
                        }
                    }
                    using (StreamWriter sw = File.AppendText(Constants.LogFilePath))
                    {
                        sw.WriteLine(logString);
                        sw.Close();
                    }
                }
                catch (Exception) { };
            }
        }   
		
		//public static string GetLastFewLogLines()
		//{
		//	string r = "";

  //          lock (logLock)
  //          {
	 //           try
	 //           {

		//            if (File.Exists(Constants.LogFilePath))
		//            {

		//            }
		//            else
		//            {
		//	            r = "Log file '" + Constants.LogFilePath + "' does not exist.";
		//            }
	 //           }
	 //           catch (Exception e)
	 //           {
		//            r = "Failed to read log file '" + Constants.LogFilePath + "'. " + e.Message;
	 //           }
  //          }

		//	return r;
		//}   

        public static void SaveSettings(long interval, string uri, int retryAfterXMinutes, bool mailServerIsOffice365, string mailServerAddress, int mailServerPort, string mailServerUsername, string mailServerPassword, string mailServerTo, string mailSubject)
        {
            lock (iniLock)
            {
                try
                {

                    List<string> linesList = new List<string>();
                    linesList.Add("Interval="+interval);
                    linesList.Add("Uri="+uri);
                    linesList.Add("RetryAfterXMinutes=" + retryAfterXMinutes);
                    linesList.Add("MailServerIsOffice365=" + mailServerIsOffice365);
                    linesList.Add("MailServerAddress=" + mailServerAddress);
                    linesList.Add("MailServerPort=" + mailServerPort);
                    linesList.Add("MailServerUsername=" + mailServerUsername);
                    linesList.Add("MailServerPassword=" + mailServerPassword);
                    linesList.Add("MailServerTo=" + mailServerTo);
                    linesList.Add("MailSubject=" + mailSubject);

                    File.WriteAllLines(Constants.IniFilePath, linesList.ToArray());
                }
                catch (Exception exception)
                {
                    WriteLog(DateTime.Now + "Error occured while writeing to file " + Constants.IniFilePath + ". Error details:" + exception.Message);
                }
            }
        }

        public static long GetInterval()
        {
            long r = 30*60*1000;//30min

            lock (iniLock)
            {
                try
                {
                    if (File.Exists(Constants.IniFilePath))
                    {
						r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<long>("Interval", Constants.IniFilePath, false, r, false, r, true, r, '=');
					}
                }
                catch (Exception exception)
                {
                    WriteLog(DateTime.Now + "Error occured while reading Interval from " + Constants.IniFilePath + ". Error details:" + exception.Message);
                }
            }

            return r;
        }

        public static string GetUri()
        {
            string r = @"http://f4cio.com/HealthStatus.aspx";

            try
            {

                if (File.Exists(Constants.IniFilePath))
                {
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("Uri", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
            }
            catch (Exception exception)
            {
                WriteLog(DateTime.Now + "Error occured while reading Uri from "+Constants.IniFilePath+". Error details:"+exception.Message);
            }

            return r;
        }

		public static int GetRetryAfterXMinutes()
		{
			int r =10;//10min

			lock (iniLock)
			{
				try
				{
					if (File.Exists(Constants.IniFilePath))
					{
						r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<int>("RetryAfterXMinutes", Constants.IniFilePath, false, r, false, -1, true, r, '=');
					}
				}
				catch (Exception exception)
				{
					WriteLog(DateTime.Now + "Error occured while reading RetryAfterXMinutes from " + Constants.IniFilePath + ". Error details:" + exception.Message);
				}
			}

			return r;
		}

		public static bool GetMailServerIsOffice365()
		{
			bool r = true;

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<bool>("MailServerIsOffice365", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailServerPort from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}

		public static string GetMailServerAddress()
		{
			string r = @"smtp.office365.com";

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailServerAddress", Constants.IniFilePath, false, r, false, r, true, r,'=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailServerAddress from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}

		public static int GetMailServerPort()
		{
			int r = 587;

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<int>("MailServerPort", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailServerPort from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}

		public static string GetMailServerUsername()
		{
			string r = "ncurcin@craftsynth.com";

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailServerUsername", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailServerUsername from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}
		
		public static string GetMailServerPassword()
		{
			string r = "";

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailServerPassword", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailServerPassword from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}

		public static string GetMailServerTo()
		{
			string r = "f4cio@yahoo.com";

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailServerTo", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailServerTo from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}
		
		public static string GetMailSubject()
		{
			string r = "Server Down";

			try
			{

				if (File.Exists(Constants.IniFilePath))
				{
					r = CraftSynth.BuildingBlocks.IO.FileSystem.GetSettingFromIniFile<string>("MailSubject", Constants.IniFilePath, false, r, false, r, true, r, '=');
				}
			}
			catch (Exception exception)
			{
				WriteLog(DateTime.Now + "Error occured while reading MailSubject from " + Constants.IniFilePath + ". Error details:" + exception.Message);
			}

			return r;
		}

		public static string RequestUri(string updateUri)
        {
            string responseString = null;

            try
            {
                WriteLog(DateTime.Now + " Requesting '" + updateUri + "'...");

                // used to build entire input
                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateUri);

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                responseString = sb.ToString();
            }
            catch (Exception exception)
            {
                WriteLog(DateTime.Now + " Requesting of '" + updateUri + "' failed. Error details:" + exception.Message);
            }

            WriteLog(DateTime.Now + " Requesting succeeded.");
            return responseString;
        }

        public static void SaveResponseHtm(string responseString)
        {
            lock (htmLock)
            {
                try
                {
                    File.WriteAllText(Constants.HtmFilePath, responseString);
                }
                catch (Exception exception)
                {
                    WriteLog(DateTime.Now + "Error occured while saving response string to " + Constants.HtmFilePath + ". Error details:" + exception.Message);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;

namespace F4CIOsDNSUpdaterCommon
{
	public class Threads
	{
		public static Thread StartWorkingThread()
		{
			Thread newThread = new Thread(new ThreadStart(ThreadLoop));
			newThread.Start();
			return newThread;
		}

		public static void ThreadLoop()
		{
			while (true)
			{
				long intervalInMiliseconds = DataAccess.GetInterval();

				Thread.Sleep(Convert.ToInt32(intervalInMiliseconds));

				Threads.DoJob(null, MailAlertShouldBeSent.OnlyIfUrlFailed, null, null, null, null, null, null, null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uri">pass null to use value from .ini file</param>
		/// <returns></returns>
		public static string DoJob(string uri, MailAlertShouldBeSent mailAlertShouldBeSent, bool? mailServerIsOffice365, string mailServerAddress, int? mailServerPort, string mailServerUsername, string mailServerPassword, string mailServerTo, string mailSubject)
		{
			string response = string.Empty;
			string error = null;

			uri = uri ?? DataAccess.GetUri();
			int retryAfterXMinutes = DataAccess.GetRetryAfterXMinutes();
			mailServerIsOffice365 = mailServerIsOffice365 ?? DataAccess.GetMailServerIsOffice365();
			mailServerAddress = mailServerAddress ?? DataAccess.GetMailServerAddress();
			mailServerPort = mailServerPort ?? DataAccess.GetMailServerPort();
			mailServerUsername = mailServerUsername ?? DataAccess.GetMailServerUsername();
			mailServerPassword = mailServerPassword ?? DataAccess.GetMailServerPassword();
			mailServerTo = mailServerTo ?? DataAccess.GetMailServerTo();
			mailSubject = mailSubject ?? DataAccess.GetMailSubject();

			bool ok = IsHealthy(uri, ref response, ref error);

			if (!ok && retryAfterXMinutes > 0)
			{
				DataAccess.WriteLog("Waiting "+retryAfterXMinutes+" minutes before retry...");
				Thread.Sleep(retryAfterXMinutes*60*1000);
				ok = IsHealthy(uri, ref response, ref error);
			}

			bool sendMailAlert;
			switch (mailAlertShouldBeSent)
			{
				case MailAlertShouldBeSent.InAnyCase:
					sendMailAlert = true;
					break;
				case MailAlertShouldBeSent.Never:
					sendMailAlert = false;
					break;
				case MailAlertShouldBeSent.OnlyIfUrlFailed:
					sendMailAlert = !ok;
					break;
				default:
					throw new ArgumentOutOfRangeException("mailAlertShouldBeSent");
			}

			if (sendMailAlert)
			{
				CustomTraceLog customTraceLog = new CustomTraceLog("Sending Mail...");
				try
				{
					MailMessage mail = new MailMessage(mailServerUsername, mailServerTo, mailSubject, "Log:");
					if (response.IsNOTNullOrWhiteSpace())
					{
						mail.Body = response;
						mail.IsBodyHtml = true; 
					}
					else
					{
						mail.Body = error.ToNonNullString();
						mail.IsBodyHtml = false;
					}
					CraftSynth.BuildingBlocks.IO.EMail.SendMail(mailServerIsOffice365.Value, mailServerAddress, mailServerPort, mailServerUsername, mailServerPassword, mail, customTraceLog);
				}
				catch (Exception e)
				{
					customTraceLog.AddLine("Error occured while sending mail. Error details:" + e.Message);
					DataAccess.WriteLog(customTraceLog.ToString());
				}
			}

			return response;
		}

		private static bool IsHealthy(string uri, ref string response, ref string error)
		{
			bool ok = false;

			try
			{
				response = DataAccess.RequestUri(uri);

				if (!string.IsNullOrEmpty(response))
				{
					if (response.RemoveRepeatedSpaces().Length < 255)
					{
						response = response.Trim().Replace("<br/>", "; ").Replace("\r", "").Replace("\n", "").RemoveRepeatedSpaces().Trim();
						DataAccess.WriteLog(DateTime.Now + " Response recieved:" + response);

						int hMailServerRunning = int.Parse(response.GetSubstring("hMailServer running:", ";"));
						long freeMbOnC = long.Parse(response.GetSubstring("C:", " MB"));
						long freeMbOnD = long.Parse(response.GetSubstring("D:", " MB"));

						if (hMailServerRunning == 1 && freeMbOnC > 100 && freeMbOnD > 100)
						{
							ok = true;
						}
					}

					DataAccess.SaveResponseHtm(response);
				}
			}
			catch (Exception exception)
			{
				error = DateTime.Now + " Error occured while requesting response and writing LastResponse.htm . Error details:" + exception.Message;
				DataAccess.WriteLog(error);
			}

			return ok;
		}
	}

	public enum MailAlertShouldBeSent
	{
		InAnyCase,
		Never,
		OnlyIfUrlFailed
	}
}

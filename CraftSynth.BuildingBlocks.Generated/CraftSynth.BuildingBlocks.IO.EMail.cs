using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
//using System.Web.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using CraftSynth.BuildingBlocks.Logging;
using CBB_CommonExtenderClass = CraftSynth.BuildingBlocks.Common.ExtenderClass;
using CBB_Logging = CraftSynth.BuildingBlocks.Logging;

namespace CraftSynth.BuildingBlocks.IO
{
	public class EMail
	{
		/// <summary>
		/// Sends email by using configuration from:
		/// location: HttpContext.Current.Request.ApplicationPath
		/// from:     .config 
		/// from tag: system.net/mailSettings
		/// </summary>
		/// <param name="mailMessage"></param>
		public static void SendEMail(MailMessage mailMessage)
		{
			Configuration config = null;
			try
			{
				config = ConfigurationManager.OpenExeConfiguration("/web.config");
			}
			catch
			{
				try
				{
					config = ConfigurationManager.OpenExeConfiguration(Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "web.config"));
				}
				catch
				{
					throw;
					//try
					//{
					//	config = ConfigurationManager.OpenExeConfiguration(Path.Combine(HttpContext.Current.Request.ApplicationPath, "web.config"));
					//}
					//catch
					//{
					//try
					//{
					//	config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/web.config");
					//}
					//catch
					//{
					//	try
					//	{
					//		config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "web.config"));
					//	}
					//	catch
					//	{
					//		try
					//		{
					//			config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(Path.Combine(HttpContext.Current.Request.ApplicationPath, "web.config"));
					//		}
					//		catch
					//		{
					//			throw;
					//		}
					//	}
					//}
					//}
				}
			}
			
			MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
			SmtpClient smtpc = new SmtpClient(settings.Smtp.Network.Host, settings.Smtp.Network.Port);
			smtpc.Send(mailMessage);
		}

		public static void SendMailViaFlowSmtpServer(string SmtpHostname, int SmtpPort, string SmtpUsername, string SmtpPassword, MailMessage mailMessage)
		{
			SmtpClient smtpClient = new SmtpClient(SmtpHostname, SmtpPort)
			{
				UseDefaultCredentials = false,
				Credentials = new System.Net.NetworkCredential(SmtpUsername, SmtpPassword)
			};
			smtpClient.Send(mailMessage);
		}

		public static void SendMail(bool viaOffice365, string smtpHostname, int? smtpPort, string smtpUsername, string smtpPassword, MailMessage mail, object customTraceLog = null)
		{
			var log = CBB_Logging.CustomTraceLog.Unbox(customTraceLog);
			string recipients = string.Empty;
			if (log != null)
			{
				List<string> recipientsList = new List<string>();
				foreach (MailAddress mailAddress in mail.To)
				{
					recipientsList.Add(mailAddress.Address);
				}
				recipients = CBB_CommonExtenderClass.ToCSV(recipientsList);
			}

			if (!viaOffice365)
			{
				SmtpClient smtp = new SmtpClient();
				smtp.Host = smtpHostname;
				if (smtpPort.HasValue)
				{
					smtp.Port = smtpPort.Value;
				}
				//smtp.UseDefaultCredentials = true;
				smtp.EnableSsl = false;
				smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
				smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				using (log.LogScope("Sending mail to '" + recipients + "' ..."))
				{
					smtp.Send(mail);
				}
			}
			else
			{
				if (mail.From == null)
				{
					mail.From = new MailAddress(smtpUsername);
				}

				if (string.Compare(mail.From.Address, smtpUsername, StringComparison.OrdinalIgnoreCase) != 0)
				{
					throw new Exception(string.Format("Office365 will not allow mail relay with different Office365 username and mail's From field. Username: {0}, From: {1}", smtpUsername, mail.From.Address));
				}

				SmtpClient smtpClient = new SmtpClient(smtpHostname, smtpPort.Value)
				{
					UseDefaultCredentials = false,
					EnableSsl = true,
					//DeliveryMethod = SmtpDeliveryMethod.Network,
					Credentials = new NetworkCredential(smtpUsername, smtpPassword)
				};

				using (log.LogScope("Sendig mail to '" + recipients + "' via Office365 ..."))
				{
					smtpClient.Send(mail);
				}
			}
		}

	}
}

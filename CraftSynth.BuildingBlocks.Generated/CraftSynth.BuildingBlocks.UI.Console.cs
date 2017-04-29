using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using CraftSynth.BuildingBlocks.Common;
using Microsoft.Win32;

using CBB_Logging = CraftSynth.BuildingBlocks.Logging;

namespace CraftSynth.BuildingBlocks.UI
{
	public class Console
	{
		/// <summary>
		/// Example: C:\App1
		/// </summary>
		public static string ApplicationPhysicalPath
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase).Replace("file:\\", string.Empty);
			}
		}

		/// <summary>
		/// Example: C:\App1\app1.exe
		/// </summary>
		public static string ApplicationPhysicalExeFilePath
		{
			get
			{
				//.Location not tested!!
				return Assembly.GetEntryAssembly().Location.Replace("file:\\", string.Empty);
			}
		}

		/// <summary>
		/// Example: C:\App1\app1
		/// </summary>
		public static string ApplicationPhysicalExeFilePathWithoutExtension
		{
			get
			{
				//.Location not tested!!
				string r = ApplicationPhysicalExeFilePath;
				r = Path.ChangeExtension(r, string.Empty);
				r = r.TrimEnd('.');
				return r;
			}
		}

		/// <summary>
		/// Gets visibility of "...exe has stopped working" dialog.
		/// Source: http://itgeorge.net/disabling-has-stopped-working-dialogs/
		/// </summary>
		/// <returns></returns>
		public static bool? GetUnhandledExceptionDialogEnabledState()
		{
			bool? enabled = null;

			object exceptionsDialog = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting", "DontShowUI", null);
			if (exceptionsDialog == null)
			{
				enabled = null;
			}
			else
			{
				int dontShowUIValue = (int)exceptionsDialog;
				enabled = dontShowUIValue != 0;
			}

			return enabled;
		}

		/// <summary>
		/// Controls visibility of "...exe has stopped working" dialog. Null will delete registry key. See GetUnhandledExceptionDialogEnabledState().
		/// Source: http://itgeorge.net/disabling-has-stopped-working-dialogs/
		/// </summary>
		/// <param name="enabled"></param>
		public static void SetUnhandledExceptionDialogEnabledState(bool? enabled)
		{
			object exceptionsDialog = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting", "DontShowUI", null);

			if (enabled == null)
			{
				if (exceptionsDialog == null)
				{
					//key does not exist -do nothing
				}
				else
				{
					//key exist but because enabled==null was passed we need to delete it
					using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting", true))
					{
						if (key == null)
						{
							throw new Exception("non null was retrieved but now key does not exist.");
						}
						else
						{
							key.DeleteValue("DontShowUI");
						}
					}
				}
			}
			else 
			{
				Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting", "DontShowUI", enabled.Value ? 1 : 0);
			}
		}

		/// <summary>
		/// Pass null for parameters if none exist.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <param name="timeoutInMilliseconds"></param>
		/// <param name="workingFolder"></param>
		/// <param name="log"></param>
		/// <param name="logCommandAndParametersBeforeExecution"></param>
		/// <param name="disableUnhandledExceptionDialog"></param>
		/// <returns></returns>
		public static int ExecuteCommand(string command, string parameters, int timeoutInMilliseconds, string workingFolder = null, CBB_Logging.CustomTraceLog log = null, bool logCommandAndParametersBeforeExecution = true, bool disableUnhandledExceptionDialog = true, string outputPartThatWillForceExitCode0 = null, string outputPartThatWillForceExitCodeMinus1 = null)
		{
			if (log == null)
			{
				log = new CBB_Logging.CustomTraceLog();
			}
			CBB_Logging.CustomTraceLog outputLog = new CBB_Logging.CustomTraceLog();

			if (logCommandAndParametersBeforeExecution)
			{
				CBB_Logging.CustomTraceLogExtensions.AddLineAndIncreaseIdent(log, "Command: " + command + " " + parameters.ToNonNullString());
			}
			else
			{
				CBB_Logging.CustomTraceLogExtensions.AddLineAndIncreaseIdent(log, "Command execution preparation...");
			}

			bool? oldStateOfUnhandledExceptionDialog = null;
			if (disableUnhandledExceptionDialog)
			{
				oldStateOfUnhandledExceptionDialog = GetUnhandledExceptionDialogEnabledState();
				//log.AddLine("State of UnhandledExceptionDialog (DontShowUI in registry):"+(oldStateOfUnhandledExceptionDialog.HasValue?oldStateOfUnhandledExceptionDialog.ToString():"null"));
				//log.AddLine("State of UnhandledExceptionDialog (DontShowUI in registry) will be set to true temporarly...");
				SetUnhandledExceptionDialogEnabledState(true);
				//log.AddLine("State of UnhandledExceptionDialog (DontShowUI in registry) changed.");
			}

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			try
			{
				if (string.IsNullOrEmpty(workingFolder))
				{
					workingFolder = CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath;
				}

				process.StartInfo.WorkingDirectory = workingFolder;
				process.StartInfo.UseShellExecute = false;
				//process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
				process.StartInfo.FileName = Path.Combine(workingFolder, command);
				//process.StartInfo.Arguments = "/C "+ command+" "+parameters;//m_gpg.HomePath + "\\gpg2.exe --import " + Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "interelate.asc");
				if (!string.IsNullOrEmpty(parameters))
				{
					process.StartInfo.Arguments = parameters;
				}
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.ErrorDialog = false;
				//si.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;

				StringBuilder output = new StringBuilder();
				StringBuilder error = new StringBuilder();
				using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
				using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
				{
					process.OutputDataReceived += (sender, e) =>
					                              {
						                              if (e.Data == null)
						                              {
							                              outputWaitHandle.Set();
						                              }
						                              else
						                              {
							                              CBB_Logging.CustomTraceLogExtensions.AddLine(log, ">> " + e.Data);
														  CBB_Logging.CustomTraceLogExtensions.AddLine(outputLog, e.Data);
							                              output.AppendLine(e.Data);
						                              }
					                              };
					process.ErrorDataReceived += (sender, e) =>
					                             {
						                             if (e.Data == null)
						                             {
							                             errorWaitHandle.Set();
						                             }
						                             else
						                             {
														 CBB_Logging.CustomTraceLogExtensions.AddLine(log,">> " + e.Data);
														 CBB_Logging.CustomTraceLogExtensions.AddLine(outputLog, e.Data);
							                             error.AppendLine(e.Data);
						                             }
					                             };

					CBB_Logging.CustomTraceLogExtensions.AddLine(log, "Command executing...");
					process.Start();

					process.BeginOutputReadLine();
					process.BeginErrorReadLine();

					if (process.WaitForExit(timeoutInMilliseconds) &&
					    outputWaitHandle.WaitOne(timeoutInMilliseconds) &&
					    errorWaitHandle.WaitOne(timeoutInMilliseconds))
					{
						// Process completed. Check process.ExitCode here.
						//log.AddLine("Done.");
					}
					else
					{
						// Timed out.
						CBB_Logging.CustomTraceLogExtensions.AddLine(log, string.Format("Operation timed out. (timeout={0})", timeoutInMilliseconds));
					}
				}
			}
			finally 
			{
				if (disableUnhandledExceptionDialog)
				{
					//log.AddLine("Recovering state of UnhandledExceptionDialog (DontShowUI in registry)...");
					SetUnhandledExceptionDialogEnabledState(oldStateOfUnhandledExceptionDialog);
					//log.AddLine("Recovered.");
				}
			}


			CBB_Logging.CustomTraceLogExtensions.AddLine(log, "ExitCode: " + process.ExitCode);
			int r = process.ExitCode;

			if (!string.IsNullOrEmpty(outputPartThatWillForceExitCode0) || !string.IsNullOrEmpty(outputPartThatWillForceExitCodeMinus1))
			{
				List<string> outputLogLines = outputLog.ToString().Split('\n').ToList();

				//chack only part of log that represent output for success and failure indicators
				if (outputLogLines.Count > 0)
				{
					for (int i = outputLogLines.Count - 1; i >= 0; i--)
					{
						if (!string.IsNullOrEmpty(outputPartThatWillForceExitCode0) && outputLogLines[i].Contains(outputPartThatWillForceExitCode0))
						{
							CBB_Logging.CustomTraceLogExtensions.AddLine(log, "Setting ExitCode to 0 because this indicator was found in output:" + outputPartThatWillForceExitCode0);
							r = 0;
							break;
						}

						if (!string.IsNullOrEmpty(outputPartThatWillForceExitCodeMinus1) && outputLogLines[i].Contains(outputPartThatWillForceExitCodeMinus1))
						{
							CBB_Logging.CustomTraceLogExtensions.AddLine(log, "Setting ExitCode to -1 because this indicator was found in output:" + outputPartThatWillForceExitCodeMinus1);
							r = -1;
							break;
						}
					}
				}
			}

			CBB_Logging.CustomTraceLogExtensions.DecreaseIdent(log);
			return r;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

using CBB_UIConsole = CraftSynth.BuildingBlocks.UI.Console;
using CBB_UIWindowsFormsMisc = CraftSynth.BuildingBlocks.UI.WindowsForms.Misc;

namespace CraftSynth.BuildingBlocks.Common
{
	public class Misc
	{
		public enum ApplicationType
		{
			Console,
			Forms,
			AspNet
		}

		/// <summary>
		/// Works 99%. See: http://stackoverflow.com/questions/3751554/application-openforms-count-0-always
		/// Source: http://stackoverflow.com/questions/5464979/determine-whether-running-in-asp-net-or-winforms-console-without-system-web
		/// </summary>
		/// <returns></returns>
		public static ApplicationType GetApplicationType()
		{
			ApplicationType r;
			if (Process.GetCurrentProcess().ProcessName.ToLower().Contains("w3wp") ||
			    Process.GetCurrentProcess().ProcessName.ToLower().Contains("aspnet_wp") ||
			    Process.GetCurrentProcess().ProcessName.ToLower().Contains("iisexpress")
				)
			{
				r = ApplicationType.AspNet;
			}
			else if (Application.OpenForms.Count > 0)
			{
				r = ApplicationType.Forms;
			}
			else
			{
				r = ApplicationType.Console;
			}
			return r;
		}

		public static string ApplicationRootFolderPath
		{
			get
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}
		}

		public static string ApplicationPhysicalExeFilePathWithoutExtension
		{

			get
			{
				string r;
				switch (GetApplicationType())
				{
					case ApplicationType.Console:
						r = CBB_UIConsole.ApplicationPhysicalExeFilePathWithoutExtension;
						break;
					case ApplicationType.Forms:
						r = CBB_UIWindowsFormsMisc.ApplicationPhysicalExeFilePathWithoutExtension;
						break;
					case ApplicationType.AspNet:
						throw new Exception("AspNet has no main exe file.");
						break;
					default:
						throw new Exception(
							"AddTimestampedExceptionInfoToApplicationWideLog: Application type not supported. Application type:" +
							GetApplicationType());
				}
				return r;
			}
		}

		public static string version
		{
			get
			{
				try
				{
					Assembly _assemblyInfo =
					Assembly.GetExecutingAssembly();

					string ourVersion = String.Empty;

					//if running the deployed application, you can get the version
					//  from the ApplicationDeployment information. If you try
					//  to access this when you are running in Visual Studio, it will
					//not work.
					if
					(ApplicationDeployment.IsNetworkDeployed)
					{
						ourVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
					}
					else
					{
						if (_assemblyInfo != null)
						{
							ourVersion = _assemblyInfo.GetName().Version.ToString();
						}
					}
					return ourVersion;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public static KeyValuePair<long,long> GetConsumedMemoryAndConsumedMemoryOfManagedPart()
		{
			KeyValuePair<long,long> r = new KeyValuePair<long, long>(0,0);

			Process currentProcess = Process.GetCurrentProcess();
			long consumedManagedMemory = GC.GetTotalMemory(true);
			currentProcess.Refresh();
			long consumedMemory = currentProcess.PrivateMemorySize64;

			r = new KeyValuePair<long, long>(consumedMemory,consumedManagedMemory);
			return r;
		}

		public static KeyValuePair<long, long> GetDeltaOfMemoryConsumption( KeyValuePair<long, long> consumptionBefore)
		{
			KeyValuePair<long,long> r = new KeyValuePair<long, long>(0,0);

			Process currentProcess = Process.GetCurrentProcess();
			long consumedManagedMemory = GC.GetTotalMemory(true);
			currentProcess.Refresh();
			long consumedMemory = currentProcess.PrivateMemorySize64;

			r = new KeyValuePair<long, long>(consumedMemory - consumptionBefore.Key, consumedManagedMemory - consumptionBefore.Value);
			return r;
		}

		public static long GetWorkingSetSizeInBytes()
		{
			Process currentProcess = Process.GetCurrentProcess();
			currentProcess.Refresh();
			long r = currentProcess.WorkingSet64;
			return r;
		}

		public static long GetVirtualMemorySizeInBytes()
		{
			Process currentProcess = Process.GetCurrentProcess();
			currentProcess.Refresh();
			long r = currentProcess.VirtualMemorySize64;
			return r;
		}



		//Can return this list:
		// v2.0.50727,2.0.50727.4927,SP2
		// v3.0,3.0.30729.4926,SP2
		// v3.5,3.5.30729.4926,SP1
		// v4,
		// v4,Client,4.5.50938
		// v4,Full,4.5.50938
		// v4.0,
		// v4.0,Client,4.0.0.0
		public static List<string> GetDotNetVersionsInstalledFromRegistry()
		{
			List<string> r = new List<string>();
			using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, String.Empty).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
			{
				foreach (string versionKeyName in ndpKey.GetSubKeyNames())
				{
					if (versionKeyName.StartsWith("v"))
					{

						RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
						string name = (string)versionKey.GetValue("Version", "");
						string sp = versionKey.GetValue("SP", "").ToString();
						string install = versionKey.GetValue("Install", "").ToString();
						if (install == "")
						{
							//no install info, ust be later
							r.Add(versionKeyName + "," + name);
						}
						else
						{
							if (sp != "" && install == "1")
							{
								r.Add(versionKeyName + "," + name + ",SP" + sp);
							}

						}
						if (name != "")
						{
							continue;
						}
						foreach (string subKeyName in versionKey.GetSubKeyNames())
						{
							RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
							name = (string)subKey.GetValue("Version", "");
							if (name != "")
								sp = subKey.GetValue("SP", "").ToString();
							install = subKey.GetValue("Install", "").ToString();
							if (install == "")
							{
								//no install info, ust be later
								r.Add(versionKeyName + "," + name);
							}
							else
							{
								if (sp != "" && install == "1")
								{
									r.Add(versionKeyName + "," + subKeyName + "," + name + ",SP" + sp);
								}
								else if (install == "1")
								{
									r.Add(versionKeyName + "," + subKeyName + "," + name);
								}

							}

						}

					}
				}
			}
			return r;
		}

		/// <summary>
		/// Returns list of all available enum values.
		/// </summary>
		/// <typeparam name="T">Type of enum</typeparam>
		/// <returns></returns>
		public static List<T> GetEnumValues<T>()
		{
			Type enumType = typeof(T);

			List<T> values = new List<T>();

			var fields = from field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
						 where field.IsLiteral
						 select field;

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(enumType);
				values.Add((T)value);
			}

			return values;
		}

		/// <summary>
		/// Generates a random string with the given length
		/// </summary>
		/// <param name="size">Size of the string</param>
		/// <param name="lowerCase">If true, generate lowercase string</param>
		/// <returns>Random string</returns>
		public static string RandomString(int size, bool lowerCase)
		{
			StringBuilder builder = new StringBuilder();
			Random random = new Random();
			char ch;
			for (int i = 0; i < size; i++)
			{
				if (random.NextDouble() > 0.5)  // mix chars and nums
				{
					ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
				}
				else
				{
					ch = random.Next(0, 9).ToString()[0];
				}

				builder.Append(ch);
			}

			if (lowerCase)
				return builder.ToString().ToLower();

			return builder.ToString();
		}

		public static Mutex GetExistingOrCreateNewMutex(string mutexName)
		{
			if (mutexName.Contains('\\'))
			{
				throw new Exception("Mutex name can not contain backslash character after Global\\ or Local\\. Mutex name="+mutexName);
			}
			Mutex theMutex = null;
			// unique id for global mutex - Global prefix means it is global to the machine
			mutexName = String.Format("Global\\{{{0}}}", mutexName);
			try
			{
				theMutex = Mutex.OpenExisting(mutexName);
			}
			catch (WaitHandleCannotBeOpenedException)
			{
			}

			if (theMutex == null)
			{
				//Source: http://axltweek.blogspot.hu/2011/05/mutex-throw-exception-access-to-path-is.html
				var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
				var mutexsecurity = new MutexSecurity();
				mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
				
				bool isCreatedNew;
				theMutex = new Mutex(false, mutexName, out isCreatedNew, mutexsecurity);
			}
			return theMutex;
		}

		public static bool TryToAcquireMutexButDontWaitAndIgnoreAbandonedMutexEx(Mutex mutex)
		{
			bool mutexWasAquired = false;
			try
			{
				mutexWasAquired = mutex.WaitOne(new TimeSpan(0, 0, 0, 0, 1), true);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The wait completed due to an abandoned mutex"))
				{
					mutexWasAquired = true;
				}
				else
				{
					throw;
				}
			}
			return mutexWasAquired;
		}

		public static bool AcquireMutex(Mutex mutex, TimeSpan timeout, bool ignoreAbandonedMutexEx)
		{
			bool mutexWasAquired = false;
			try
			{
				mutexWasAquired = mutex.WaitOne(timeout);
			}
			catch (Exception e)
			{
				if (ignoreAbandonedMutexEx && e.Message.Contains("The wait completed due to an abandoned mutex"))
				{
					mutexWasAquired = true;
				}
				else
				{
					throw;
				}
			}
			return mutexWasAquired;
		}

		public static void BeepInNewThread(int frequency = 5000, int durationInMiliseconds = 1000)
		{
			Thread t = new Thread(() => { Console.Beep(frequency, durationInMiliseconds); });
			t.Start();
		}

		public static List<Process> GetProcessesByNamePart(string namePart)
		{
			Process[] allProcesses = Process.GetProcesses();
			List<Process> processesThatContainPhraseInName = new List<Process>();
			foreach (Process processToCheck in allProcesses)
			{
				if (-1 != processToCheck.ProcessName.IndexOf(namePart, StringComparison.InvariantCultureIgnoreCase))
				{
					processesThatContainPhraseInName.Add(processToCheck);
				}
			}

			return processesThatContainPhraseInName;
		}

		public static Exception GetDeepestException(Exception ex)
		{
			Exception r = ex;
			while (r.InnerException != null)
			{
				r = r.InnerException;
			}
			return r;
		}

		public static List<Exception> GetInnerExceptions(Exception e)
		{
			List<Exception> list = new List<Exception>();
			Exception r = e;
			list.Add(r);
			while (r.InnerException != null)
			{
				r = r.InnerException;
				list.Add(r);
			}
			list.Reverse();
			return list;
		}

		public static int CalculateMiddleInRegardsToProgress(int a, int b, float progressAsOto1, CalculateMiddleInRegardsToProgress_Function function = CalculateMiddleInRegardsToProgress_Function.Linear)
		{
			int r;

			if (a == b)
			{
				r = a;
			}
			else
			{
				if (a > b)
				{
					int t = a;
					a = b;
					b = t;
					progressAsOto1 = 1 - progressAsOto1;
				}

				switch (function)
				{
					case CalculateMiddleInRegardsToProgress_Function.Linear:
						r = (int)Math.Round(a + (b - a) * progressAsOto1);
						break;
					case CalculateMiddleInRegardsToProgress_Function.Sigmoid:
						//source: http://dynamicnotions.blogspot.rs/2008/09/sigmoid-function-in-c.html
						r = (int)Math.Round(a + (b - a) *  (1 / (1 + Math.Exp(-(progressAsOto1*12-6)))));
						break;
					default:
						throw new ArgumentOutOfRangeException("function");
				}
			}

			return r;
		}
	}

	public enum CalculateMiddleInRegardsToProgress_Function
	{
		Linear,
		Sigmoid
	}


	//System.Diagnostics.Process si = new System.Diagnostics.Process();
	//si.StartInfo.WorkingDirectory = m_gpg.HomePath;
	//si.StartInfo.UseShellExecute = false;
	//si.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
	//si.StartInfo.Arguments = "dir";//m_gpg.HomePath + "\\gpg2.exe --import " + Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, "interelate.asc");
	//si.StartInfo.CreateNoWindow = true;
	////si.StartInfo.RedirectStandardInput = true;
	//si.StartInfo.RedirectStandardOutput = true;
	//si.StartInfo.RedirectStandardError = true;
	//si.Start();

	//int timeout = 2000;
	//StringBuilder output = new StringBuilder();
	//StringBuilder error = new StringBuilder();
	//using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
	//using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
	//{
	//	si.OutputDataReceived += (sender, e) =>
	//	{
	//		if (e.Data == null)
	//		{
	//			outputWaitHandle.Set();
	//		}
	//		else
	//		{
	//			output.AppendLine(e.Data);
	//		}
	//	};
	//	si.ErrorDataReceived += (sender, e) =>
	//	{
	//		if (e.Data == null)
	//		{
	//			errorWaitHandle.Set();
	//		}
	//		else
	//		{
	//			error.AppendLine(e.Data);
	//		}
	//	};

	//	si.Start();

	//	si.BeginOutputReadLine();
	//	si.BeginErrorReadLine();

	//	if (si.WaitForExit(timeout) &&
	//		outputWaitHandle.WaitOne(timeout) &&
	//		errorWaitHandle.WaitOne(timeout))
	//	{
	//		// Process completed. Check process.ExitCode here.
	//	}
	//	else
	//	{
	//		// Timed out.
	//	}
	//}
}

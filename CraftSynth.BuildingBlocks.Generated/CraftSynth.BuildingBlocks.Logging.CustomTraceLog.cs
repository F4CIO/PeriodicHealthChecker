using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftSynth.BuildingBlocks.Logging
{	
	/// <summary>
	/// Sole purpose of this class is to help to build string that represents processing steps.
	/// String can be included in ex message.
	/// 
	/// Result example:
	/// Started login process...
	///   Checking pass...
	///     Success..
	///   Checing roles..
	///     Sucess..
	/// Login done.
	/// </summary>
	public class CustomTraceLog
	{
		private StringBuilder _sb;
		private DateTime? _timestamp;
		private bool _prependTimestamps;
		private bool _useUtcForTimestamps;
		private int _ident;
		private string _oneIdent;
		private SensitiveStringsList _sensitiveStringsList = new SensitiveStringsList();
		
		private string CalculateIdent(int ident)
		{
			string result = string.Empty;
			for (int i = 0; i < ident; i++)
			{
				result += this._oneIdent;
			}
			return result;
		}

		public delegate void AddLinePostProcessingEventDelegate(CustomTraceLog sender, string line, bool inNewLine, int level, string lineVersionSuitableForLineEnding, string lineVersionSuitableForNewLine);
		private event AddLinePostProcessingEventDelegate _AddLinePostProcessingEvent;

		public delegate void AddLinePreProcessingEventDelegate(CustomTraceLog  sender, ref string line, ref bool inNewLine, ref int level);
		private event AddLinePreProcessingEventDelegate _AddLinePreProcessingEvent;

		public delegate DateTime DateTimeNowReplacementFunctionDelegate(bool shouldReturnInUtc);
		private event DateTimeNowReplacementFunctionDelegate _dateTimeNowReplacementFunction;
		
		public object Tag { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="initialValue"></param>
		/// <param name="prependTimestamps"> </param>
		/// <param name="useUtcForTimestamps"> </param>
		/// <param name="customTraceLog_AddLinePostProcessingEvent">
		/// You and set function to be called automatically after each AddLine call. Function format is:
		/// void CustomTraceLog_AddLinePostProcessingEvent(string line){}
		/// </param>
		/// <param name="customTraceLog_AddLinePreProcessingEvent">
		/// You can set function to be called automatically before each AddLine call. Use it to detect and hide passwords for example. Function format is:
		/// void CustomTraceLog_AddLinePostProcessingEvent(string line){}
		/// </param>
		public CustomTraceLog(string initialValue = null, bool prependTimestamps = true, bool useUtcForTimestamps = false, AddLinePostProcessingEventDelegate customTraceLog_AddLinePostProcessingEvent = null, AddLinePreProcessingEventDelegate customTraceLog_AddLinePreProcessingEvent = null, object tag = null, string oneIdent = "  ", DateTimeNowReplacementFunctionDelegate dateTimeNowReplacementFunction = null)
		{
			_sb = new StringBuilder(initialValue ?? string.Empty);
			this._prependTimestamps = prependTimestamps;
			this._useUtcForTimestamps = useUtcForTimestamps;
			_ident = 0;
			if (customTraceLog_AddLinePreProcessingEvent != null)
			{
				this._AddLinePreProcessingEvent += customTraceLog_AddLinePreProcessingEvent;
			}
			if (customTraceLog_AddLinePostProcessingEvent != null)
			{
				this._AddLinePostProcessingEvent += customTraceLog_AddLinePostProcessingEvent;
			}
			this.Tag = tag;
			this._oneIdent = oneIdent;
			this._dateTimeNowReplacementFunction = dateTimeNowReplacementFunction;
			//HandlerForLogFile.Append(this.ToString().TrimEnd('\n').TrimEnd('\r').TrimEnd('n')+"\r\n");
			this._AddLine(initialValue);
		}

		public static CustomTraceLog Unbox(object customTraceLog)
		{
			CustomTraceLog log = null;
			if (customTraceLog != null)
			{
				if (!(customTraceLog is CustomTraceLog))
				{
					throw new Exception("customTraceLog param must be of type CustomTraceLog.");
				}
				else
				{
					log = (CustomTraceLog)customTraceLog;
				}
			}
			return log;
		}

		internal void _IncreaseIdent()
		{
			this._ident++;
		}

		internal void _DecreaseIdent()
		{
			if (this._ident > 0)
			{
				_ident--;
			}
		}

		public void _AssignSensitiveString(string sensitiveString, int replacementType = (int)SensitiveStringReplacementType.Replace50PercentInMiddle, string replacementString = "...hidden...")
		{
			SensitiveString ss = new SensitiveString(sensitiveString, replacementType, replacementString);
			this._sensitiveStringsList.AddOrReplace(ss);
		}

		internal void _AddLine(string message, bool inNewLine = true, int level = 0)
		{
			if (this._AddLinePreProcessingEvent != null)
			{
				this._AddLinePreProcessingEvent.Invoke(this, ref message, ref inNewLine, ref level);
			}

			message = this._sensitiveStringsList.ReplaceAllSensitiveStrings(message);


			string timestamp = string.Empty;
			if (this._prependTimestamps)
			{
				if (this._dateTimeNowReplacementFunction == null)
				{
					this._timestamp = this._useUtcForTimestamps ? DateTime.UtcNow : DateTime.Now;
				}
				else
				{
					this._timestamp = this._dateTimeNowReplacementFunction.Invoke(this._useUtcForTimestamps);
				}

				timestamp = string.Format("{0}.{1}.{2} {3}:{4}:{5} ({6}) ",
							  this._timestamp.Value.Year,
							  this._timestamp.Value.Month.ToString().PadLeft(2, '0'),
							  this._timestamp.Value.Day.ToString().PadLeft(2, '0'),
							  this._timestamp.Value.Hour.ToString().PadLeft(2, '0'),
							  this._timestamp.Value.Minute.ToString().PadLeft(2, '0'),
							  this._timestamp.Value.Second.ToString().PadLeft(2, '0'),
							  this._useUtcForTimestamps ? "UTC" : "local");
			}
			string lineVersionSuitableForLineEnding = message;
			string lineVersionSuitableForNewLine = timestamp + this.CalculateIdent(this._ident+(inNewLine?0:1)) + message;
			string line = inNewLine ? timestamp + this.CalculateIdent(this._ident) + message : message;

			if (inNewLine)
			{
				_sb.Append("\r\n"+line);
			}
			else
			{
				_sb.Append(line);
			}

			if (this._AddLinePostProcessingEvent != null)
			{
				this._AddLinePostProcessingEvent.Invoke(this, line, inNewLine, level, lineVersionSuitableForLineEnding, lineVersionSuitableForNewLine);
			}
		}

		public string GetVersionForNewLine;
		
		internal void _AddLineAndIncreaseIdent(string message, bool inNewLine = true, int level = 0)
		{
			this._AddLine(message, inNewLine, level);
			this._IncreaseIdent();
		}

		internal void _AddLineAndDecreaseIdent(string message, bool inNewLine = true, int level = 0)
		{
			this._AddLine(message, inNewLine, level);
			this._DecreaseIdent();
		}
		
		//public void ExtendLastLine(string message)
		//{
		//	_sb.Append(message);
		//}

		//public void AddLine(int ident,string message)
		//{
		//	_sb.AppendLine(this.CalculateIdent(ident) + message);
		//}

		public override string ToString()
		{
			return _sb.ToString();
		}

		public int Length
		{
			get { return _sb.Length; }
		}

		public string[] ToLines()
		{
			return _sb.ToString().Split(new string[] {"\r\n", "\n", "\r"}, StringSplitOptions.None);
		}
	}

	public static class CustomTraceLogExtensions
	{
		public static void IncreaseIdent(this CustomTraceLog log)
		{
			if (log != null)
			{
				log._IncreaseIdent();
			}
		}

		public static void DecreaseIdent(this CustomTraceLog log)
		{
			if (log != null)
			{
				log._DecreaseIdent();
			}
		}

		public static void AddLine(this CustomTraceLog log, string message, bool inNewLine = true, int level = 0)
		{
			if (log != null)
			{
				log._AddLine(message, inNewLine, level);
			}
		}

		public static void AddLineAndIncreaseIdent(this CustomTraceLog log, string message, bool inNewLine = true, int level = 0)
		{
			if (log != null)
			{
				log._AddLineAndIncreaseIdent(message, inNewLine, level);
			}
		}

		public static void AddLineAndDecreaseIdent(this CustomTraceLog log, string message, bool inNewLine = true, int level = 0)
		{
			if (log != null)
			{
				log._AddLineAndDecreaseIdent(message, inNewLine, level);
			}
		}

		public static LogScope LogScope(this CustomTraceLog log, string startMessage, string finishMessage = "done.", int level = 0)
		{
			return new LogScope(log, startMessage, finishMessage, level);
		}

		/// <summary>
		/// Marks particular string (like password) as sensitive so that whenever it appears in log it appears masked. 
		/// </summary>
		/// <param name="log"></param>
		/// <param name="sensitiveString">Sensitive information like password.</param>
		/// <param name="replacementType">ReplaceWhole = 0,Replace50PercentInMiddle = 1,ReplaceWithLengthInformation = 2</param>
		/// <param name="replacementString">Replacement mask like XXXX.</param>
		public static void AssignSensitiveString(this CustomTraceLog log, string sensitiveString, int replacementType = (int)SensitiveStringReplacementType.Replace50PercentInMiddle, string replacementString = "...hidden...")
		{
			if (log != null)
			{
				log._AssignSensitiveString(sensitiveString, replacementType, replacementString); 
			}
		}
	}


	/// <summary>
	/// Usage:
	/// 
	/// 
	/// using(log.LogScope("Preparing items to process...", "done.", 1, true))       //AddLineAndIncreaseIdent called here
	/// {
	///     //do something...
	/// }                                                                            //AddLineAndDecreaseIdent called here
	/// 
	/// 
	/// or:
	/// 
	/// 
	/// using(new LogScope(log, "Preparing items to process...", "done.", 1, true))  //AddLineAndIncreaseIdent called here
	/// {
	///     //do something...
	/// }                                                                            //AddLineAndDecreaseIdent called here
	/// </summary>
	public class LogScope : IDisposable
	{
		private CustomTraceLog _log;
		private readonly string _startMessage;
		private readonly string _finishMessage;
		private readonly int _level;
		private readonly int _totalLengthAfterStartMessage;

		public LogScope(CustomTraceLog log, string startMessage, string finishMessage = "done.", int level = 0)
		{
			if (log != null)
			{
				this._log = log;
				this._startMessage = startMessage;
				this._finishMessage = finishMessage;
				this._level = level;

				log.AddLineAndIncreaseIdent(this._startMessage, true, this._level); //AddLineAndIncreaseIdent called here

				this._totalLengthAfterStartMessage = log.Length;
			}
			else
			{
				this._disposed = true;
			}
		}

		/// <summary>
		/// Determines whether execution is in exception.
		/// Sources:
		/// http://stackoverflow.com/questions/1815492/how-to-determine-whether-a-net-exception-is-being-handled
		/// http://www.codewrecks.com/blog/index.php/2008/07/25/detecting-if-finally-block-is-executing-for-an-manhandled-exception/
		/// https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.marshal.getexceptionpointers%28VS.80%29.aspx
		/// </summary>
		/// <returns></returns>
		private Boolean IsInException()
		{
			return Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0;
		}

		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				bool thereWereSomeNewLogsAfterStartMessage = this._log.Length != _totalLengthAfterStartMessage && this._log.ToString().Substring(_totalLengthAfterStartMessage).Contains('\n');
				if (!this.IsInException())
				{
					this._log._AddLine(_finishMessage, thereWereSomeNewLogsAfterStartMessage, this._level);
				}
				else
				{
					this._log._AddLine(_finishMessage.TrimEnd('.')+" with error.", thereWereSomeNewLogsAfterStartMessage, this._level);
				}
				this._log.DecreaseIdent();

				if (disposing)
				{
					// Free any managed objects here. 
					//
				}

				// Free any unmanaged objects here. 
				//


				this._disposed = true;
			}
		}

		~LogScope()
		{
			this.Dispose(false);
		}
	}

	public class SensitiveStringsList
	{
		public List<SensitiveString> Items = new List<SensitiveString>();

		public void AddOrReplace(SensitiveString ss)
		{
			SensitiveString existingItem = this.Items.FirstOrDefault(item => item.OriginalValue == ss.OriginalValue);
			if (existingItem == null)
			{
				this.Items.Add(ss);
			}
			else
			{
				this.Items.Remove(existingItem);
				this.Items.Add(ss);
			}
		}
		public string ReplaceAllSensitiveStrings(string content)
		{
			foreach (SensitiveString sensitiveString in Items)
			{
				content = sensitiveString.ReplaceAllOccurnces(content);
			}
			return content;
		}
	}

	/// <summary>
	/// Provides container for and automatic masking for particular sensitive string like password.
	/// </summary>
	public class SensitiveString
	{
		#region Private Members

		private string _originalString;
		private int _replacementType;
		private string _replacementValue;

		#endregion

		#region Properties

		#endregion

		#region Public Methods

		public string OriginalValue
		{
			get
			{
				return _originalString;
			}
		}

		public string ReplacedValue
		{
			get
			{
				return this.ToString();
			}
		}

		public override string ToString()
		{
			string r = _originalString;

			switch (this._replacementType)
			{
				case (int)SensitiveStringReplacementType.ReplaceWhole:
					r = _replacementValue;
					break;
				case (int)SensitiveStringReplacementType.Replace50PercentInMiddle:
					if (this._originalString.Length == 0 || this._originalString.Length==1)
					{
						r = _replacementValue;
					}
					else if (this._originalString.Length == 2)
					{
						r = this._originalString[0] + this._replacementValue;
					}
					else if (this._originalString.Length == 3)
					{
						r = this._originalString[0] + this._replacementValue + this._originalString[2];
					}
					else if (this._originalString.Length == 4)
					{
						r = this._originalString[0] + this._replacementValue + this._originalString[3];
					}
					else
					{
						//l:100=s:25 => s=l*25/100=l/4
						int visiblePartLength = (int)Math.Round((double)this._originalString.Length/4);
						r = this._originalString.Substring(0, visiblePartLength) + this._replacementValue + this._originalString.Substring(this._originalString.Length - visiblePartLength);
					}
					break;
				case (int)SensitiveStringReplacementType.ReplaceWithLengthInformation:
					r = "(character count: " + this._replacementValue.Length + ")";
					break;
			}			  	
						  
			return r;
		}

		public string ReplaceAllOccurnces(string content)
		{
			content = content.Replace(this._originalString, this.ReplacedValue);
			return content;
		}

		#endregion

		#region Constructors And Initialization

		public SensitiveString(string sensitiveString, int replacementType = (int)SensitiveStringReplacementType.Replace50PercentInMiddle, string replacementValue = "...hidden...")
		{
			this._originalString = sensitiveString;
			this._replacementType = replacementType;
			this._replacementValue = replacementValue;
		}

		#endregion

		#region Deinitialization And Destructors

		#endregion

		#region Event Handlers

		#endregion

		#region Private Methods

		#endregion

		#region Helpers

		#endregion
	}

	public enum SensitiveStringReplacementType
	{
		ReplaceWhole = 0,
		Replace50PercentInMiddle = 1,
		ReplaceWithLengthInformation = 2
	}
}

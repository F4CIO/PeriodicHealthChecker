using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using ZetaLongPaths;
using CBB_CommonMisc = CraftSynth.BuildingBlocks.Common.Misc;
using SearchOption = System.IO.SearchOption;
using System.Runtime.InteropServices;

namespace CraftSynth.BuildingBlocks.IO
{
    public class FileSystem
    {
		/// <summary>
		/// method to determine is the absolute file path is a valid path
		/// </summary>
		/// <param name="path">the path we want to check</param>
		public static bool IsFilePathValid(string path)
		{
			string pattern = @"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$";
			Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			return reg.IsMatch(path);
		}

		/// <summary>
		/// Locates specified folder or file and tells if it is folder.
		/// </summary>
		/// <param name="fileOrFolderPath"></param>
		/// <returns></returns>
		public static bool? IsFolder(string fileOrFolderPath)
		{
			bool? isFolder = null;

			if (Directory.Exists(fileOrFolderPath))
			{
				isFolder = true;
			}
			else
				if (File.Exists(fileOrFolderPath))
				{
					isFolder = false;
				}
			return isFolder;
		}

		public static bool FileOrFolderDoesNotExist(string path)
		{
			return !File.Exists(path) && !Directory.Exists(path);
		}

		public static bool FileOrFolderExist(string path)
		{
			return File.Exists(path) || Directory.Exists(path);
		}
		
		public static bool FileOrFolderExistsUsingZetaForLongPaths(string path)
		{
			bool r;
						
			ZetaLongPaths.ZlpFileInfo zfi = new ZlpFileInfo(path);
			r = zfi.Exists;

			if (r == false)
			{
				ZetaLongPaths.ZlpDirectoryInfo zdi = new ZlpDirectoryInfo(path);
				r = zdi.Exists;
			}

			return r;
		}

		public static FileStream OpenReadUsingZetaLongPaths(string path)
		{
			FileStream r;

			try
			{
				r = File.OpenRead(path);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
						r = new FileStream(
							ZlpIOHelper.CreateFileHandle(
								path,
								ZetaLongPaths.Native.CreationDisposition.OpenAlways,
								ZetaLongPaths.Native.FileAccess.GenericRead,
								ZetaLongPaths.Native.FileShare.Read),
							FileAccess.Read);
				}
				else
				{
					throw;
				}
			}

			return r;		
		}

		public static void CreateFolderIfItDoesNotExist(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static void CreateFileIfItDoesNotExist(string filePath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}
			if (!File.Exists(filePath))
			{
				FileStream fs = File.Create(filePath);
				fs.Close();
			}
		}

	    public static long? GetFileSizeInBytes(string filePath, bool surpressAllErrors = true)
	    {
		    long? r = null;

		    try
		    {
			    FileInfo fi = new FileInfo(filePath);
			    r = fi.Length;
		    }
		    catch (Exception e)
		    {
			    if (surpressAllErrors)
			    {
				    r = null;
			    }
			    else
			    {
				    throw new Exception("Can not get file size for file '"+(filePath??"null")+"'. See inner exception.", e);
			    }
		    }

		    return r;
	    }

	    /// <summary>
		/// Gets list of strings where each is full path to file including filename (for example: <example>c:\dir\filename.ext</example>.
		/// </summary>
		/// <param name="folderPath">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <returns></returns>
		public static List<string> GetFilePaths(string folderPath, bool recursively = false, string searchPattern = null)
		{
			if (String.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "folderPath");

			List<string> r = new List<string>();

			try
			{
				if (Directory.Exists(folderPath))
				{
					string[] fileArray;
					if (searchPattern == null)
					{
						fileArray = Directory.GetFiles(folderPath);
					}
					else
					{
						fileArray = Directory.GetFiles(folderPath, searchPattern);
					}
					r.AddRange(fileArray);

					if (recursively)
					{
						string[] subfolders = Directory.GetDirectories(folderPath);
						foreach (string subfolder in subfolders)
						{
							List<string> subfolderFiles = GetFilePaths(subfolder, recursively, searchPattern);
							r.AddRange(subfolderFiles);
						}
					}
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
					r.AddRange(GetFilePathsUsingZetaForLongPaths(folderPath, recursively,searchPattern));
				}
				else
				{
					throw;
				}
			}

			return r;
		}

		/// <summary>
		/// Gets list of strings where each is full path to file including filename (for example: <example>c:\dir\filename.ext</example>.
		/// </summary>
		/// <param name="folderPath">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <returns></returns>
		private static List<string> GetFilePathsUsingZetaForLongPaths(string folderPath, bool recursively = false, string searchPattern = null)
		{
			if (String.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "folderPath");

			List<string> r = new List<string>();
			
			ZetaLongPaths.ZlpDirectoryInfo zdi = new ZlpDirectoryInfo(folderPath);
			if (zdi.Exists)
			{
				ZlpFileInfo[] files = null;
				if (searchPattern == null)
				{
					files = zdi.GetFiles(SearchOption.AllDirectories);
				}
				else
				{
					files = zdi.GetFiles(searchPattern, SearchOption.AllDirectories);
				}
				foreach (ZlpFileInfo zlpFileInfo in files)
				{
					r.Add(zlpFileInfo.FullName);
				}

				if (recursively)
				{
					ZlpDirectoryInfo[] subfolders = zdi.GetDirectories(folderPath);
					List<string> subfolderFiles = null;
					foreach (ZlpDirectoryInfo subfolder in subfolders)
					{
						subfolderFiles = GetFilePathsUsingZetaForLongPaths(subfolder.FullName, recursively, searchPattern);
						r.AddRange(subfolderFiles);
					}
				}
			}

			return r;
		}

		/// <summary>
		/// Gets list of strings where each is full path to file including filename (for example: <example>c:\dir\filename.ext</example>) or is full path of folder. Bool value is true if folder is returned.
		/// </summary>
		/// <param name="folderPath">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <returns></returns>
		public static List<KeyValuePair<string, bool>> GetFileAndFolderPaths(string folderPath, bool includeFolderPathFromFirstParameterInResult = false, bool includeFilesInResult = true, bool includeFoldersInResult = true, bool recursively = false, string searchPattern = null, bool callerIsThisSameMethod = false)
		{
			if (String.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "folderPath");

			List<KeyValuePair<string, bool>> r = new List<KeyValuePair<string, bool>>();

			try
			{
				if (Directory.Exists(folderPath))
				{
					if (!callerIsThisSameMethod)
					{
						if (includeFolderPathFromFirstParameterInResult)
						{
							r.Add(new KeyValuePair<string, bool>(folderPath, true));
						}
					}else if (includeFoldersInResult)
					{
						r.Add(new KeyValuePair<string, bool>(folderPath, true));
					}

					if (includeFilesInResult)
					{
						string[] fileArray;
						if (searchPattern == null)
						{
							fileArray = Directory.GetFiles(folderPath);
						}
						else
						{
							fileArray = Directory.GetFiles(folderPath, searchPattern);
						}
						foreach (string filePath in fileArray)
						{
							r.Add(new KeyValuePair<string, bool>(filePath, false));
						}
					}

					string[] subfolders = Directory.GetDirectories(folderPath);
					foreach (string subfolder in subfolders)
					{
						if (!recursively)
						{
							if (includeFoldersInResult)
							{
								r.Add(new KeyValuePair<string, bool>(subfolder, true));
							}
						}
						else
						{
							var subfolderItems = GetFileAndFolderPaths(subfolder, true, includeFilesInResult, includeFoldersInResult, recursively, searchPattern, true);
							r.AddRange(subfolderItems);
						}
					}
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
					throw new Exception("The specified path, file name, or both are too long. Currently processing folder:"+folderPath);
				}
				else
				{
					throw;
				}
			}

			return r;
		}

		/// <summary>
		/// Gets list of strings where each is full path to folder (for example FolderA) including foldername (for example: <example>c:\dir\FolderA</example>.
		/// </summary>
		/// <param name="folder">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static List<string> GetFolderPaths(string folderPath, bool recursively = false)
		{
			if (String.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "folderPath");

			List<string> r = new List<string>();

			string[] folderPathStrings = Directory.GetDirectories(folderPath);
			if (folderPathStrings != null)
			{
				r.AddRange(folderPathStrings);
			}

			if (recursively)
			{
				foreach (string subfolder in folderPathStrings)
				{
					List<string> innerFolders = GetFolderPaths(subfolder, recursively);
					r.AddRange(innerFolders);
				}
			}

			return r;
		}

		/// <summary>
		/// Finds all empty folders and subfolders and returns list of strings where each is full path to folder (for example: <example>c:\dir</example>.
		/// </summary>
		/// <param name="folderPath">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <returns></returns>
		public static List<string> GetEmptyFoldersPathsRecursively(string folderPath)
		{
			List<string> r = new List<string>();

			if (Directory.Exists(folderPath))
			{
				string[] fileArray = Directory.GetFiles(folderPath);
				string[] subfolders = Directory.GetDirectories(folderPath);

				if (fileArray.Length + subfolders.Length == 0)
				{
					r.Add(folderPath);
				}
				else
				{
					foreach (string subfolder in subfolders)
					{
						List<string> emptySubfolders = GetEmptyFoldersPathsRecursively(subfolder);
						r.AddRange(emptySubfolders);
					}
				}
			}

			return r;
		}

		//        public static byte[] ReadBytesFromFile(string filePath)
		//        {
		//            StreamReader rdr = File.OpenText(@"C:\boot.ini");rdr.CurrentEncoding
		//            Console.Write(rdr.Re.ReadToEnd());
		//rdr.Close()
		//        }


		//        public static string ReadTextFromFile(string filePath)
		//        {
		//            if (string.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "filePath");

		//            File.ReadAllText(
		//        }

		/// <summary>
		/// Examples: for 'C:\aaa\bbb' returns 'C:\aaa'; for 'C:\aaa\bbb.txt' (which can be file or folder) returns 'C:\aaa' .
		/// </summary>
		/// <param name="fullCurrentFolderPath"></param>
		/// <returns></returns>
		public static string GetParentFolderPath(string fullCurrentFolderPath)
		{
			string parentFolderPath = null;

			parentFolderPath = Path.GetDirectoryName(fullCurrentFolderPath);

			return parentFolderPath;
		}

		/// <summary>
		/// Writes provided sequence of bytes to file specified by filepath.
		/// </summary>
		/// <param name="filePath">Full path to file. Example: <example>c:\subdir1\file1.bin</example> </param>
		/// <param name="data"></param>
		/// <exception cref="ArgumentException">Thrown when parameter <paramref>filePath</paramref> is null or empty.</exception>
		/// <exception cref="ArgumentException">Thrown when parameter <paramref>data</paramref> is null.</exception>
		public static void WriteBytesToFile(string filePath, byte[] data)
		{
			if (String.IsNullOrEmpty(filePath)) throw new ArgumentException("Value must be non-empty string.", "filePath");
			if (data == null) throw new ArgumentNullException("data");


			FileStream fileStream = new FileStream(filePath, FileMode.CreateNew);
			BinaryWriter writer = new BinaryWriter(fileStream);

			try
			{
				fileStream.Write(data, 0, data.Length);
			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					writer.Close();
					fileStream.Close();
				}
				catch { }
			}

		}

		public static void CopyFileOrFolder(string sourcePath, string destinationPath)
		{
			destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
			if (File.Exists(sourcePath) && !Directory.Exists(sourcePath))
			{//copy file

				Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(sourcePath, destinationPath, UIOption.AllDialogs, UICancelOption.ThrowException);
			}
			else
				if (!File.Exists(sourcePath) && Directory.Exists(sourcePath))
				{//copy folder
					Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(sourcePath, destinationPath, UIOption.AllDialogs, UICancelOption.ThrowException);
				}
		}

		public static void MoveFileOrFolder(string sourcePath, string destinationPath)
		{
			destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
			if (File.Exists(sourcePath) && !Directory.Exists(sourcePath))
			{//copy file                
				Microsoft.VisualBasic.FileIO.FileSystem.MoveFile(sourcePath, destinationPath, UIOption.AllDialogs, UICancelOption.ThrowException);
			}
			else
				if (!File.Exists(sourcePath) && Directory.Exists(sourcePath))
				{//copy folder
					Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(sourcePath, destinationPath, UIOption.AllDialogs, UICancelOption.ThrowException);
				}
		}

		public static void DeleteFileOrFolder(string path, bool useRecycleBin)
		{
			DeleteFileOrFolder(path, useRecycleBin, false);
		}

		public static void DeleteFileOrFolder(string path, bool useRecycleBin, bool displayOnlyErrorDialogs)
		{
			UIOption uIOption = (displayOnlyErrorDialogs) ? UIOption.OnlyErrorDialogs : UIOption.AllDialogs;

			if (File.Exists(path) && !Directory.Exists(path))
			{//delete file
				Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path, uIOption, useRecycleBin ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently, UICancelOption.ThrowException);
			}
			else
				if (!File.Exists(path) && Directory.Exists(path))
				{//delete folder
					Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(path, uIOption, useRecycleBin ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently, UICancelOption.ThrowException);
				}
		}

		public static void DeleteFileOrFolderUsingZetaForLongPaths(string path, bool useRecycleBin, bool preferVBShellMethod, bool displayOnlyErrorDialogs)
		{
			UIOption uIOption = (displayOnlyErrorDialogs) ? UIOption.OnlyErrorDialogs : UIOption.AllDialogs;

			if (useRecycleBin)
			{
				preferVBShellMethod = true;
			}

			if (preferVBShellMethod && File.Exists(path))
			{//delete file
				Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path, uIOption, useRecycleBin ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently, UICancelOption.ThrowException);
			}
			else
			if(preferVBShellMethod && Directory.Exists(path))
			{//delete folder
				Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(path, uIOption, useRecycleBin ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently, UICancelOption.ThrowException);
			}
			else
			{				
				ZetaLongPaths.ZlpFileInfo zfi = new ZlpFileInfo(path);
				if (zfi.Exists)
				{
					if (useRecycleBin)
					{
						throw new Exception("Folder/file has too long path. Try deleting it without using Recycle Bin. Path: " + path);
					}
					else
					{
						zfi.Delete();
					}
				}
				else
				{
					ZetaLongPaths.ZlpDirectoryInfo zdi = new ZlpDirectoryInfo(path);
					if (zdi.Exists)
					{
						if (useRecycleBin)
						{
							throw new Exception("Folder/file has too long path. Try deleting it without using Recycle Bin. Path: " + path);
						}
						else
						{
							zdi.Delete(true);
						}
					}
				}				
			}
		}

		/// <summary>
		/// Source: http://stackoverflow.com/questions/3282418/send-a-file-to-the-recycle-bin
		/// </summary>
		private class FileToRecycleBinMover
		{
			/// <summary>
			/// Possible flags for the SHFileOperation method.
			/// </summary>
			[Flags]
			internal enum FileOperationFlags : ushort
			{
				/// <summary>
				/// Do not show a dialog during the process
				/// </summary>
				FOF_SILENT = 0x0004,
				/// <summary>
				/// Do not ask the user to confirm selection
				/// </summary>
				FOF_NOCONFIRMATION = 0x0010,
				/// <summary>
				/// Delete the file to the recycle bin.  (Required flag to send a file to the bin
				/// </summary>
				FOF_ALLOWUNDO = 0x0040,
				/// <summary>
				/// Do not show the names of the files or folders that are being recycled.
				/// </summary>
				FOF_SIMPLEPROGRESS = 0x0100,
				/// <summary>
				/// Surpress errors, if any occur during the process.
				/// </summary>
				FOF_NOERRORUI = 0x0400,
				/// <summary>
				/// Warn if files are too big to fit in the recycle bin and will need
				/// to be deleted completely.
				/// </summary>
				FOF_WANTNUKEWARNING = 0x4000,
			}

			/// <summary>
			/// File Operation Function Type for SHFileOperation
			/// </summary>
			internal enum FileOperationType : uint
			{
				/// <summary>
				/// Move the objects
				/// </summary>
				FO_MOVE = 0x0001,
				/// <summary>
				/// Copy the objects
				/// </summary>
				FO_COPY = 0x0002,
				/// <summary>
				/// Delete (or recycle) the objects
				/// </summary>
				FO_DELETE = 0x0003,
				/// <summary>
				/// Rename the object(s)
				/// </summary>
				FO_RENAME = 0x0004,
			}



			/// <summary>
			/// SHFILEOPSTRUCT for SHFileOperation from COM
			/// </summary>
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
			private struct SHFILEOPSTRUCT
			{

				public IntPtr hwnd;
				[MarshalAs(UnmanagedType.U4)]
				public FileOperationType wFunc;
				public string pFrom;
				public string pTo;
				public FileOperationFlags fFlags;
				[MarshalAs(UnmanagedType.Bool)]
				public bool fAnyOperationsAborted;
				public IntPtr hNameMappings;
				public string lpszProgressTitle;
			}

			[DllImport("shell32.dll", CharSet = CharSet.Auto)]
			private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

			private class ErrorTableItem
			{
				public string ID;
				public int ErrorCode;
				public string Message;
			}

			/// <summary>
			/// Source: https://msdn.microsoft.com/en-us/library/windows/desktop/bb762164(v=vs.85).aspx
			/// </summary>
			private static List<ErrorTableItem> ErrorTable
			{
				get
				{
					List<ErrorTableItem> items = new List<ErrorTableItem>();
					items.Add(new ErrorTableItem() { ID = "DE_SAMEFILE		         ", ErrorCode = 0x71, Message = "The source and destination files are the same file.																										   " });
					items.Add(new ErrorTableItem() { ID = "DE_MANYSRC1DEST	         ", ErrorCode = 0x72, Message = "Multiple file paths were specified in the source buffer, but only one destination file path.																   " });
					items.Add(new ErrorTableItem() { ID = "DE_DIFFDIR			     ", ErrorCode = 0x73, Message = "Rename operation was specified but the destination path is a different directory.Use the move operation instead.											   " });
					items.Add(new ErrorTableItem() { ID = "DE_ROOTDIR			     ", ErrorCode = 0x74, Message = "The source is a root directory, which cannot be moved or renamed.																							   " });
					items.Add(new ErrorTableItem() { ID = "DE_OPCANCELLED		     ", ErrorCode = 0x75, Message = "The operation was canceled by the user, or silently canceled if the appropriate flags were supplied to SHFileOperation.									   " });
					items.Add(new ErrorTableItem() { ID = "DE_DESTSUBTREE		     ", ErrorCode = 0x76, Message = "The destination is a subtree of the source.																												   " });
					items.Add(new ErrorTableItem() { ID = "DE_ACCESSDENIEDSRC        ", ErrorCode = 0x78, Message = "Security settings denied access to the source.																												   " });
					items.Add(new ErrorTableItem() { ID = "DE_PATHTOODEEP		     ", ErrorCode = 0x79, Message = "The source or destination path exceeded or would exceed MAX_PATH.																							   " });
					items.Add(new ErrorTableItem() { ID = "DE_MANYDEST		         ", ErrorCode = 0x7A, Message = "The operation involved multiple destination paths, which can fail in the case of a move operation.															   " });
					items.Add(new ErrorTableItem() { ID = "DE_INVALIDFILES	         ", ErrorCode = 0x7C, Message = "The path in the source or destination or both was invalid.																									   " });
					items.Add(new ErrorTableItem() { ID = "DE_DESTSAMETREE	         ", ErrorCode = 0x7D, Message = "The source and destination have the same parent folder.																									   " });
					items.Add(new ErrorTableItem() { ID = "DE_FLDDESTISFILE          ", ErrorCode = 0x7E, Message = "The destination path is an existing file.																													   " });
					items.Add(new ErrorTableItem() { ID = "DE_FILEDESTISFLD          ", ErrorCode = 0x80, Message = "The destination path is an existing folder.																												   " });
					items.Add(new ErrorTableItem() { ID = "DE_FILENAMETOOLONG        ", ErrorCode = 0x81, Message = "The name of the file exceeds MAX_PATH.																														   " });
					items.Add(new ErrorTableItem() { ID = "DE_DEST_IS_CDROM          ", ErrorCode = 0x82, Message = "The destination is a read-only CD-ROM, possibly unformatted.																								   " });
					items.Add(new ErrorTableItem() { ID = "DE_DEST_IS_DVD		     ", ErrorCode = 0x83, Message = "The destination is a read-only DVD, possibly unformatted.																									   " });
					items.Add(new ErrorTableItem() { ID = "DE_DEST_IS_CDRECORD       ", ErrorCode = 0x84, Message = "The destination is a writable CD-ROM, possibly unformatted.																								   " });
					items.Add(new ErrorTableItem() { ID = "DE_FILE_TOO_LARGE         ", ErrorCode = 0x85, Message = "The file involved in the operation is too large for the destination media or file system.																	   " });
					items.Add(new ErrorTableItem() { ID = "DE_SRC_IS_CDROM	         ", ErrorCode = 0x86, Message = "The source is a read-only CD-ROM, possibly unformatted.																									   " });
					items.Add(new ErrorTableItem() { ID = "DE_SRC_IS_DVD		     ", ErrorCode = 0x87, Message = "The source is a read-only DVD, possibly unformatted.																										   " });
					items.Add(new ErrorTableItem() { ID = "DE_SRC_IS_CDRECORD        ", ErrorCode = 0x88, Message = "The source is a writable CD-ROM, possibly unformatted.																										   " });
					items.Add(new ErrorTableItem() { ID = "DE_ERROR_MAX		         ", ErrorCode = 0xB7, Message = "MAX_PATH was exceeded during the operation.																												   " });
					items.Add(new ErrorTableItem() { ID = "UNKNOWN			         ", ErrorCode = 0x402, Message = "An unknown error occurred. This is typically due to an invalid path in the source or destination. This error does not occur on Windows Vista and later.	   " });
					items.Add(new ErrorTableItem() { ID = "ERRORONDEST		         ", ErrorCode = 0x10000, Message = "An unspecified error occurred on the destination.																										   " });
					items.Add(new ErrorTableItem() { ID = "DE_ROOTDIR_OR_ERRORONDEST ", ErrorCode = 0x10074, Message = " Destination is a root directory and cannot be renamed.																									   " });
					return items;
				}
			}

			private static void ThrowExceptionIfError(int r, string path)
			{
				if (r != 0)
				{
					ErrorTableItem eti = ErrorTable.FirstOrDefault(item => item.ErrorCode == r);
					if (eti == null)
					{
						throw new Exception("Unknown error occured while sending to recycle bin this file: " + path ?? "null");
					}
					else
					{
						throw new Exception("Error occured while sending to recycle bin this file: " + (path ?? "null") + " Details:" + eti.Message);
					}
				}
			}

			/// <summary>
			/// Send file to recycle bin
			/// </summary>
			/// <param name="path">Location of directory or file to recycle</param>
			/// <param name="flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
			public static void Send(string path, FileOperationFlags flags)
			{			
				var fs = new SHFILEOPSTRUCT
				{
					wFunc = FileOperationType.FO_DELETE,
					pFrom = path + '\0' + '\0',
					fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags
				};
				int r =	SHFileOperation(ref fs);//slow
				ThrowExceptionIfError(r,path);
			}

			/// <summary>
			/// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
			/// </summary>
			/// <param name="path">Location of directory or file to recycle</param>
			public static void Send(string path)
			{
				Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_WANTNUKEWARNING);
			}

			/// <summary>
			/// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
			/// </summary>
			/// <param name="path">Location of directory or file to recycle</param>
			public static void MoveToRecycleBin(string path)
			{
				Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI | FileOperationFlags.FOF_SILENT);
			}

			private static void deleteFile(string path, FileOperationFlags flags)
			{
				var fs = new SHFILEOPSTRUCT
				{
					wFunc = FileOperationType.FO_DELETE,
					pFrom = path + '\0' + '\0',
					fFlags = flags
				};
				int r = SHFileOperation(ref fs);
				ThrowExceptionIfError(r, path);
			}

			public static void DeleteCompletelySilent(string path)
			{
				deleteFile(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI | FileOperationFlags.FOF_SILENT);
			}
		}

		public static void DeleteFile(string path, bool useRecycleBin, bool deletePermanentlyIfPathToLong)
		{
			ZetaLongPaths.ZlpFileInfo zfi = new ZlpFileInfo(path);
			if (!zfi.Exists)
			{
				throw new FileNotFoundException("File not found. File path:" + path ?? "null");
			} 
			else
			{
				if (useRecycleBin)
				{
					try
					{
						FileToRecycleBinMover.MoveToRecycleBin(path);
					}
					catch (Exception e)
					{
						if (e.Message.Contains("The specified path, file name, or both are too long.")
						|| e.Message.Contains("The source or destination path exceeded or would exceed MAX_PATH")
						|| e.Message.Contains("The path in the source or destination or both was invalid")
						|| e.Message.Contains("The name of the file exceeds MAX_PATH")
						|| e.Message.Contains("MAX_PATH was exceeded during the operation")
						)
						{
							if (deletePermanentlyIfPathToLong)
							{
								zfi.Delete();
							}
							else
							{
								throw new Exception("File path too long for Recycle Bin. Try deleting it without using Recycle Bin. Path: " + path);
							}
						}
						else if (e.Message.Contains("The file involved in the operation is too large for the destination media or file system"))
						{
							throw new Exception("File too big for Recycle Bin. Try deleting it without using Recycle Bin. Path: " + path);
						}
					}
				}
				else
				{
					zfi.Delete();
				}
			}
		}

		/// <summary>
		/// Deletes folder even if it has read-only items.
		/// Source: http://caioproiete.net/en/csharp-delete-folder-including-its-sub-folders-and-files-even-if-marked-as-read-only/
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recursive"></param>
		public static void DeleteFolder(string path, bool recursive = true)
		{
			
			try
			{
				// Delete all files and sub-folders?
				if (recursive)
				{
					// Yep... Let's do this
					var subfolders = Directory.GetDirectories(path);
					foreach (var s in subfolders)
					{
						DeleteFolder(s, recursive);
					}
				}

				// Get all files of the folder
				var files = Directory.GetFiles(path);
				foreach (var f in files)
				{
					RemoveReadOnlyAttributeIfExists(path);

					// Delete the file
					File.Delete(f);
				}

				// When we get here, all the files of the folder were
				// already deleted, so we just delete the empty folder
				Directory.Delete(path);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
					DeleteFolderUsingZetaLongPaths(path, recursive);
				}
				else
				{
					throw;
				}
			}
		}

	    public static void DeleteFolderUsingZetaLongPaths(string path, bool recursive = true)
	    {
			ZetaLongPaths.ZlpDirectoryInfo di = new ZlpDirectoryInfo(path);
			// Delete all files and sub-folders?
			if (recursive)
			{
				// Yep... Let's do this
				var subfolders = di.GetDirectories(path);
				foreach (var s in subfolders)
				{
					DeleteFolderUsingZetaLongPaths(s.FullName, recursive);
				}
			}

			// Get all files of the folder
			var files = di.GetFiles(path);
			foreach (var f in files)
			{
				RemoveReadOnlyAttributeIfExistsUsingZetaLongPaths(path);

				// Delete the file
				f.Delete();
			}

			// When we get here, all the files of the folder were
			// already deleted, so we just delete the empty folder
			di.Delete(true);//TODO: should recursive simply be used? can it handle read-only items?
	    }

	    public static bool RemoveReadOnlyAttributeIfExists(string filePath)
	    {
		    bool r = false;

		    try
		    {
			    var attr = File.GetAttributes(filePath);

			    // Is this file marked as 'read-only'?
			    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			    {
				    // Yes... Remove the 'read-only' attribute, then
				    File.SetAttributes(filePath, attr ^ FileAttributes.ReadOnly);
				    r = true;
			    }
		    }
		    catch (Exception e)
		    {
			    if (e.Message.Contains("The specified path, file name, or both are too long."))
			    {
				    r = RemoveReadOnlyAttributeIfExistsUsingZetaLongPaths(filePath);
			    }
			    else
			    {
				    throw;
			    }
		    }

		    return r;
	    }

	    public static bool RemoveReadOnlyAttributeIfExistsUsingZetaLongPaths(string filePath)
	    {
		    bool r = false;
			
			ZlpFileInfo fi = new ZlpFileInfo(filePath);
		    if ((fi.Attributes & ZetaLongPaths.Native.FileAttributes.Readonly) != 0)
		    {
			    fi.Attributes = fi.Attributes & ~ZetaLongPaths.Native.FileAttributes.Readonly;
			    r = true;
		    }

		    return r;
	    }

	    public static string[] ReadAllLinesFromFile(string filePath)
	    {
		    string[] r = null;
		    
			try
		    {
			    r = File.ReadAllLines(filePath);
		    }
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
					r = ReadAllLinesFromFileUsingZetaLongPaths(filePath);
				}
				else
				{
					throw;
				}
			}

		    return r;
	    }

		public static string[] ReadAllLinesFromFileUsingZetaLongPaths(string filePath)
		{
			string[] r = null;

		    ZetaLongPaths.ZlpFileInfo fi = new ZlpFileInfo(filePath);
			r = fi.ReadAllText().Split(new string[] { "\r\n", "\n","\r" }, StringSplitOptions.None);
			
			return r;
		}

		public static byte[] ReadAllBytesFromFile(string filePath)
		{
			byte[] r = null;

			try
			{
				r = File.ReadAllBytes(filePath);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
					r = ReadAllBytesFromFileUsingZetaLongPaths(filePath);
				}
				else
				{
					throw;
				}
			}

			return r;
		}

		public static byte[] ReadAllBytesFromFileUsingZetaLongPaths(string filePath)
		{
			byte[] r = null;

			ZetaLongPaths.ZlpFileInfo fi = new ZlpFileInfo(filePath);
			r = fi.ReadAllBytes();
			
			return r;
		}

		public static void WriteAllBytesToFile(string filePath, byte[] bytes)
		{
			try
			{
				File.WriteAllBytes(filePath, bytes);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("The specified path, file name, or both are too long."))
				{
					WriteAllBytesToFileUsingZetaLongPaths(filePath, bytes);
				}
				else
				{
					throw;
				}
			}
		}

		public static void WriteAllBytesToFileUsingZetaLongPaths(string filePath, byte[] bytes)
		{
			ZetaLongPaths.ZlpIOHelper.WriteAllBytes(filePath,bytes);
		}

	    /// <summary>
		/// Reads all bytes from file specified by parameter.
		/// </summary>
		/// <param name="filePath">Full path to file from which data should be read. Example: <example>c:\subdir1\file1.bin</example></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown when parameter <paramref>filePath</paramref> is null or empty.</exception>
		public static byte[] ReadBytesFromFile(string filePath)
		{
			if (String.IsNullOrEmpty(filePath)) throw new ArgumentException("Value must be non-empty string.", "filePath");


			byte[] data = null;

			FileStream fileStream = new FileStream(filePath, FileMode.Open);
			BinaryWriter writer = new BinaryWriter(fileStream);

			try
			{
				byte[] buffer = new byte[32768];
				using (MemoryStream ms = new MemoryStream())
				{
					int read = 0;
					do
					{
						read = fileStream.Read(buffer, 0, buffer.Length);
						if (read > 0) ms.Write(buffer, 0, read);
					} while (read > 0);

					data = ms.ToArray();
				}

			}
			//catch (Exception exception)
			//{
			//    ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.DAL_Wrap_Policy);
			//}
			finally
			{
				try
				{
					writer.Close();
					fileStream.Close();
				}
				catch { }
			}

			return data;
		}

		// This method accepts two strings the represent two files to 
		// compare. A return value of 0 indicates that the contents of the files
		// are the same. A return value of any other value indicates that the 
		// files are not the same.
		public static bool CompareFileContent(string file1, string file2)
		{
			int file1byte;
			int file2byte;
			FileStream fs1;
			FileStream fs2;

			// Determine if the same file was referenced two times.
			if (file1 == file2)
			{
				// Return true to indicate that the files are the same.
				return true;
			}

			// Open the two files.
			fs1 = new FileStream(file1, FileMode.Open);
			fs2 = new FileStream(file2, FileMode.Open);

			// Check the file sizes. If they are not the same, the files 
			// are not the same.
			if (fs1.Length != fs2.Length)
			{
				// Close the file
				fs1.Close();
				fs2.Close();

				// Return false to indicate files are different
				return false;
			}

			// Read and compare a byte from each file until either a
			// non-matching set of bytes is found or until the end of
			// file1 is reached.
			do
			{
				// Read one byte from each file.
				file1byte = fs1.ReadByte();
				file2byte = fs2.ReadByte();
			}
			while ((file1byte == file2byte) && (file1byte != -1));

			// Close the files.
			fs1.Close();
			fs2.Close();

			// Return the success of the comparison. "file1byte" is 
			// equal to "file2byte" at this point only if the files are 
			// the same.
			return ((file1byte - file2byte) == 0);
		}

		/// <summary>
		/// Gets app setting from .config file.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>Value linked to specified key. 'null' if key not found or error occured.</returns>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static string GetAppSetting(string key)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentException("Value must be non-empty string.", "key");

			string result;
			try
			{
				result = ConfigurationManager.AppSettings[key];
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// Gets app setting as int from .config file.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>Value linked to specified key. 'null' if key not found or error occured.</returns>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static int? GetAppSettingAsInt(string key)
		{
			int? result;

			string value = GetAppSetting(key);

			try
			{
				result = Convert.ToInt32(value);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}




		public static string GetTargetPathFromLnkManualy(string filePath)
		{
			string fileContents = File.ReadAllText(filePath, Encoding.Unicode);
			string target = String.Empty;
			int targetStartIndex = fileContents.IndexOf("http://", 0, StringComparison.InvariantCultureIgnoreCase);
			if (targetStartIndex == -1)
			{
				targetStartIndex = fileContents.IndexOf("https://", 0, StringComparison.InvariantCultureIgnoreCase);
			}
			if (targetStartIndex == -1)
			{
				targetStartIndex = fileContents.IndexOf("ftp://", 0, StringComparison.InvariantCultureIgnoreCase);
			}
			if (targetStartIndex == -1)
			{
				targetStartIndex = fileContents.IndexOf("ftps://", 0, StringComparison.InvariantCultureIgnoreCase);
			}
			if (targetStartIndex != -1)
			{
				target = fileContents.Substring(targetStartIndex).Trim();
				target = target.Replace("\0", String.Empty);
			}
			return target;
		}
		
		public static string GetUrlFromInternetShortcut(string filePath)
		{
			var lines = File.ReadAllLines(filePath);
			string target = lines.First(line => line.ToUpper().StartsWith("URL=")).Substring(4);
			
			return target;
		}

		/// <summary>
		/// Saves url as windows internet shortcut to specified file path. 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="url"></param>
		public static void SaveAsInternetShortcut(string filePath, string url)
		{
			string[] contents = new string[] { "[InternetShortcut]", "URL=" + url };
			File.WriteAllLines(filePath, contents);
		}

		/// <summary>
		/// Increments appended number to filename until no file with same filename is found.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public static string NumberIfFileExists(string fileName)
		{
			string newFileName = fileName;

			if (File.Exists(fileName))
			{
				FileInfo fileInfo = new FileInfo(fileName);
				int dotPos = fileInfo.Name.LastIndexOf('.');
				string absFileName = fileInfo.Name.Substring(0, dotPos > 0 ? dotPos : fileInfo.Name.Length);
				int counter = 0;

				do
				{
					counter++;
					StringBuilder sb = new StringBuilder();
					sb.Append(fileInfo.Directory.FullName);
					sb.Append("\\");
					sb.Append(absFileName);
					sb.Append("_");
					sb.Append(counter.ToString());
					sb.Append(fileInfo.Extension);
					newFileName = sb.ToString();
				}
				while (File.Exists(newFileName));
			}

			return newFileName;
		}

		/// <summary>
		/// Numbers if file exists. Existance is checked in two folders.
		/// </summary>
		/// <param name="fileNameWithExt">File name in form FileName.ext</param>
		/// <param name="folderPath1">First folder full path against which to check for existance.</param>
		/// <param name="folderPath2">Second folder full path against which to check for existance.</param>
		/// <returns>Numbered file name in form 'FileName (index).ext' that is not present in either of folders.</returns>
		public static string NumberIfFileExists(string fileNameWithExt, string folderPath1, string folderPath2)
		{
			string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileNameWithExt);
			string ext = Path.GetExtension(fileNameWithExt);

			string result = fileNameWithExt;
			int i = 1;
			while (File.Exists(Path.Combine(folderPath1, result)) || File.Exists(Path.Combine(folderPath2, result)))
			{
				result = String.Format("{0} ({1}).{2}", fileNameWithoutExt, i, ext);
				i++;
			}

			return result;
		}

	    public static string GetNumberedNameIfFolderExists(string folderPath)
	    {
		    string r = folderPath;
		    int i = 1;
			
		    while (Directory.Exists(r) || File.Exists(r))
		    {
			    r = folderPath + "_" + i;
			    i++;

			    if (i >= int.MaxValue)
			    {
				    throw new Exception("Can not determine name for folder: "+folderPath);
			    }
		    }

		    return r;
	    }

		public static void AppendFile(string filePath, string text)
		{
			using (var sw = new StreamWriter(filePath, true))
			{
				sw.Write(text);
				sw.Flush();
			}
		}

		public enum ConcurrencyProtectionMechanism
		{
			None,
			Lock,
			Mutex
		}

	    private static Dictionary<string, object> _appendFileLockObject;
		public static void AppendFile(string filePath, string text, ConcurrencyProtectionMechanism concurrencyProtectionMechanism, string mutexName = null)
		{
			switch (concurrencyProtectionMechanism)
			{
				case ConcurrencyProtectionMechanism.None:
					AppendFile(filePath, text);
					break;
				case ConcurrencyProtectionMechanism.Lock:
					if (_appendFileLockObject == null)
					{
						_appendFileLockObject = new Dictionary<string, object>();
					}
					if (!_appendFileLockObject.ContainsKey(filePath.ToUpper()))
					{
						_appendFileLockObject.Add(filePath.ToUpper(), new object());
					}
					lock (_appendFileLockObject[filePath.ToUpper()])
					{
						AppendFile(filePath, text);
					}
					break;
				case ConcurrencyProtectionMechanism.Mutex:
					if (String.IsNullOrEmpty(mutexName))
					{
						mutexName = "CBB_AppendFileMutex-" + filePath.ToUpper().Replace('\\','_');
						//throw new ArgumentException("When ConcurencyProtectionMechanism is mutex mutexName must be specified.");
					}
					Mutex theMutex = CBB_CommonMisc.GetExistingOrCreateNewMutex(mutexName);
					theMutex.WaitOne();
					try
					{
						AppendFile(filePath, text);
					}
					finally
					{
						theMutex.ReleaseMutex();
					}
					break;
				default: throw new Exception(String.Format("ConcurrencyProtectionMechanism '{0}' not recognised.", concurrencyProtectionMechanism));
			}
		}
		
		public static void WaitForMutexAndRenameFile(string mutexName, TimeSpan timeToWaitBeforeThrowingException, string oldFilePath, string newFilePath, bool overwrite)
		{
			Mutex mutex = CBB_CommonMisc.GetExistingOrCreateNewMutex(mutexName);
			if (!mutex.WaitOne(timeToWaitBeforeThrowingException,true))//new TimeSpan(0, 0, 60)))
			{
				throw new Exception(String.Format("WaitForMutexAndRenameFile: Mutex {0} was locked more than {1}. That should not happen.", mutexName, timeToWaitBeforeThrowingException));
			}
			else
			{
				try
				{
					if (!File.Exists(oldFilePath))
					{
						throw new Exception("WaitForMutexAndRenameFile: Source file not found: " + oldFilePath);
					}

					if (!overwrite && File.Exists(newFilePath))
					{
						throw new Exception(String.Format("WaitForMutexAndRenameFile: File {0} already exists. Overwriting not allowed by design.", newFilePath));
					}

					if (overwrite && File.Exists(newFilePath))
					{
						File.Delete(newFilePath);
					}

					File.Move(oldFilePath, newFilePath);

				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// For C:\MyDir\MySubDir\myfile.ext returns:
		/// C:\ 
		/// C:\MyDir\
		/// C:\MyDir\MySubDir
		/// C:\MyDir\MySubDir\myfile.ext
		///  
		/// Source: http://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k%28System.IO.Path.GetDirectoryName%29;k%28TargetFrameworkMoniker-.NETFramework
		/// </summary>
		/// <param name="path"></param>
		/// <param name="includePathItself"></param>
		/// <returns></returns>
	    public static List<string> GetAncestorsFullPaths(string path, bool includeRoot, bool includePathItself, bool orderIsFromRootToDeepest)
	    {
		    List<string> r = new List<string>();
			string directoryName;
			int i = 0;

			while (path != null)
			{
				directoryName = Path.GetDirectoryName(path);
				//handle check for dipest folder
				if ((i==0 && includePathItself) || i>0)
				{
					//handle check for root 
					if (directoryName!=null || (directoryName == null && includePathItself))
					{
						r.Add(path);
					}
					
				}
				path = directoryName;
				//if (i == 1)
				//{
				//	path = directoryName + @"\";  // this will preserve the previous path
				//}
				i++;
			}

			if (orderIsFromRootToDeepest)
			{
				r.Reverse();
			}

			return r;
	    }

		public static List<string> GetAllChildrensAndDescendants(string path, List<string> allPaths, bool includeItselfInResult, bool includeNonFirstLevelDescendants, bool performSimpleAlphanumericSortOnFinalList)
		{
			List<string> r = new List<string>();

			path = path.ToLower().TrimEnd('\\');
			foreach (string itm in allPaths)
			{
				string item = itm.TrimEnd('\\');
				if (includeItselfInResult && item.Length == path.Length && string.Equals(item, path, StringComparison.OrdinalIgnoreCase))
				{//add itself to result 
					r.Add(item);
				}
				else if (path.Length < item.Length && item.ToLower().StartsWith(path))
				{//add descendent if neccessary
					bool firstLevelDescendant = !item.Substring(path.Length + 1).Contains("\\");
					if (firstLevelDescendant || includeNonFirstLevelDescendants)
					{
						r.Add(item);
					}
				}
			}

			if (performSimpleAlphanumericSortOnFinalList)
			{
				r.Sort();
			}

			return r;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="settingName"></param>
		/// <param name="filePath">Pass null to use .ini file with same name as executable.</param>
		/// <param name="throwErrorIfNotFound"></param>
		/// <param name="resultInNotFoundCase"></param>
		/// <param name="throwErrorIfEmptyString"></param>
		/// <param name="resultInEmptyStringCase"></param>
		/// <param name="hideErrors"></param>
		/// <param name="resultInErrorCase"></param>
		/// <param name="separatorBetweenKeyAndValue"></param>
		/// <returns></returns>
		public static T GetSettingFromIniFile<T>(string settingName, string filePath, bool throwErrorIfNotFound, T resultInNotFoundCase, bool throwErrorIfEmptyString, T resultInEmptyStringCase, bool hideErrors, T resultInErrorCase, char separatorBetweenKeyAndValue = '=')
	    {
		    T r = default(T);

		    settingName = settingName.Trim().ToLower();
		    try
		    {
			    bool found = false;

				if (string.IsNullOrEmpty(filePath))
				{
					filePath = CBB_CommonMisc.ApplicationPhysicalExeFilePathWithoutExtension + ".ini";
				}

			    List<string> lines = File.ReadAllLines(filePath).ToList();
			    foreach (string line in lines)
			    {
				    if (line.Trim().ToLower().StartsWith(settingName))
				    {
					    string key = line.Split(separatorBetweenKeyAndValue)[0];
					    if (key.Trim().ToLower() == settingName)
					    {
						    found = true;
						    string value = String.Empty;
						    if (line.Trim().Length > key.Length + 1)
						    {
							    value = line.Substring(key.Length + 1);
							    value = value.Trim();
						    }

						    if (value == String.Empty)
						    {
							    if (!throwErrorIfEmptyString)
							    {
								    r = resultInEmptyStringCase;
							    }
							    else
							    {
									string errorMessage = string.Format("Empty value for setting '{0}' in file '{1}' is not allowed.", settingName ?? "null", filePath ?? "null");
									if (resultInEmptyStringCase != null)
									{
										errorMessage = resultInErrorCase.ToString();
									}
									throw new Exception(errorMessage);
							    }
						    }
						    else
						    {
							    r = (T)Convert.ChangeType(value, typeof(T));
						    }

						    break;
					    }
				    }
			    }

			    if (found == false)
			    {
				    if (!throwErrorIfNotFound)
				    {
						r = resultInNotFoundCase;
					}
				    else
				    {
					   string errorMessage = string.Format("Missing setting '{0}' from file '{1}'.", settingName ?? "null", filePath??"null");
					    if (resultInNotFoundCase!=null)
					    {
						    errorMessage = resultInEmptyStringCase.ToString();
					    }
						throw new Exception(errorMessage);
				    }
			    }

		    }
		    catch (Exception exception)
		    {
			    if (hideErrors)
			    {
				    r = resultInErrorCase;
			    }
			    else
			    {
				    Exception outerException = new Exception(String.Format("Can not read setting '{0}' from file '{1}'.", settingName ?? "null", filePath ?? "null"), exception);
				    throw outerException;
			    }
		    }
		    return r;
	    }

		//[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		//[return: MarshalAs(UnmanagedType.Bool)]
		//static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
		//									   out ulong lpFreeBytesAvailable,
		//									   out ulong lpTotalNumberOfBytes,
		//									   out ulong lpTotalNumberOfFreeBytes);
		//public static long GetFreeDiskSpaceInBytes(string fileOfFolderUncPath)
		//{
		//	ulong FreeBytesAvailable;
		//	ulong TotalNumberOfBytes;
		//	ulong TotalNumberOfFreeBytes;

		//	bool success = GetDiskFreeSpaceEx(@"\\mycomputer\myfolder",
		//								out FreeBytesAvailable,
		//								out TotalNumberOfBytes,
		//								out TotalNumberOfFreeBytes);
		//	if (!success)
		//		throw new System.ComponentModel.Win32Exception();
		//}

		public static double GetTotalSpaceForDriveByFileOrFolder(string fileOrFolderPath, DiskSizeUnit unit)
	    {
		    string  driveLetter = Path .GetPathRoot(fileOrFolderPath);
			DriveInfo  drive = new  DriveInfo (driveLetter);
		    var space = GetTotalSpaceForDrive(drive, unit);
		    return space;
	    }

	    public static double GetTotalSpaceForDrive(DriveInfo driveInfo, DiskSizeUnit sizeUnit)
	    {
		    double freeSpace = -1;
		    double formatDivideBy = 1;

		    if (driveInfo != null)
		    {
			    long freeSpaceNative = driveInfo.TotalFreeSpace;
			    formatDivideBy = Math.Pow(1024, (int) sizeUnit);
			    freeSpace = freeSpaceNative/formatDivideBy;
		    }

		    return freeSpace;
	    }   

		public static double GetFreeSpaceForDriveByFileOrFolder(string fileOrFolderPath, DiskSizeUnit unit)
	    {
		    string  driveLetter = Path.GetPathRoot(fileOrFolderPath);
			DriveInfo  drive = new  DriveInfo (driveLetter);
		    var space = GetFreeSpaceForDrive(drive, unit);
		    return space;
	    }

	    public static double GetFreeSpaceForDrive(DriveInfo driveInfo, DiskSizeUnit sizeUnit)
	    {
		    double freeSpace = -1;
		    double formatDivideBy = 1;

		    if (driveInfo != null)
		    {
			    long freeSpaceNative = driveInfo.AvailableFreeSpace;
			    formatDivideBy = Math.Pow(1024, (int) sizeUnit);
			    freeSpace = freeSpaceNative/formatDivideBy;
		    }

		    return freeSpace;
	    }
    }
	public  enum  DiskSizeUnit
	{
		Bytes = 0,
		KiloBytes = 1,
		MegaBytes = 2,
		GigaBytes = 3,
		TeraBytes = 4
	}
}

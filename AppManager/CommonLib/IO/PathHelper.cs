﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace CommonLib
{
	public static class PathHelper
	{
        private static readonly IEnumerable<string> s_pathContens;
        private static readonly string s_winDirPath;


        static PathHelper()
        {
            s_winDirPath = Environment.ExpandEnvironmentVariables("%WINDIR%");

            var path = Environment.GetEnvironmentVariable("PATH");
			if (string.IsNullOrEmpty(path))
            {
				s_pathContens = new string[0];
            }
            else
            {
                s_pathContens = path.Split(';');
            }
        }


		/// <summary>
		/// Trying to locate file in system dirs
		/// </summary>
		/// <remarks>
		///	1. Windows system directory
		///	2. Windows directory
		///	3. The directories that are listed in the PATH
		/// </remarks>
		/// <returns></returns>
		public static string ResolveFilePath(string fileFullName)
		{ 
			var dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
			if (File.Exists(Path.Combine(dir, fileFullName)))
			{
				return dir;
			}

            dir = s_winDirPath;
			if (File.Exists(Path.Combine(dir, fileFullName)))
			{
				return dir;
			}

            foreach (var pathDir in s_pathContens)
			{
				if (File.Exists(Path.Combine(pathDir, fileFullName)))
				{
					return pathDir;
				}
			}

			return null;
		}

		public static string GetFirstDrivePath()
		{ 
			var drvs = DriveInfo.GetDrives();
			foreach (var item in drvs)
			{
				if (item.DriveType == DriveType.Fixed)
					return item.RootDirectory.FullName;
			}

			return String.Empty;
		}

		public static string GetExistingPath(string dir)
		{
			if (Directory.Exists(dir))
				return dir;

			while (!String.IsNullOrEmpty(dir))
			{
				dir = PathHelper.GetUpperPath(dir);
				if (Directory.Exists(dir))
					break;
			}

			return dir;
		}

		public static bool ComparePath(string path1, string path2)
		{
			if (String.IsNullOrEmpty(path1) &&
				String.IsNullOrEmpty(path2))
				return path1 == path2;

			if (String.IsNullOrEmpty(path1) ||
				String.IsNullOrEmpty(path2))
				return false;

			path1 = Environment.ExpandEnvironmentVariables(path1.Trim('"'));
			path2 = Environment.ExpandEnvironmentVariables(path2.Trim('"'));

			return String.Equals(path1, path2, StringComparison.CurrentCultureIgnoreCase);
		}

		public static string GetFilePathFromExecPath(string path, out string args)
		{
			args = String.Empty;
			string tempPath = path;

			if (tempPath != null)
				tempPath = tempPath.Trim(' ', '\"');

			if (String.IsNullOrEmpty(tempPath))
				return String.Empty;

			if (!tempPath.Contains("\"") && (File.Exists(tempPath) || Directory.Exists(tempPath)))
				return tempPath;

			string trimPath = tempPath;

			while (true)
			{
				int lix = Math.Max(
					tempPath.LastIndexOf(' '),
					tempPath.LastIndexOf(Path.DirectorySeparatorChar));
				if (lix < 0)
					break;

				tempPath = tempPath.Substring(0, lix);
				tempPath = tempPath.Trim('\"');

				if (!HasInvalidChars(tempPath) && Directory.Exists(tempPath))
					return trimPath;

				if (!HasInvalidChars(tempPath) && !tempPath.Contains("\"") && File.Exists(tempPath))
					break;
			}

			string argsTemp = path.Trim(' ', '\"');
			argsTemp = argsTemp.Substring(tempPath.Length, argsTemp.Length - tempPath.Length);
			argsTemp = argsTemp.Trim(' ', '\"');
			args = argsTemp;

			if (!Directory.Exists(tempPath) && !FileExists(tempPath))
				return trimPath;

			return tempPath;

			//args = String.Empty;
			//string path = ExecPath;

			//if (path != null)
			//   path = path.Trim();

			//if (String.IsNullOrEmpty(path))
			//   return String.Empty;
			////Path.GetDirectoryName(path)
			//int lix = path.LastIndexOf('"');
			//string filePath;
			//if (lix >= 0)
			//{
			//   filePath = path.Substring(0, lix);
			//   filePath = filePath.Trim();
			//   filePath = filePath.Trim('\"');
			//   filePath = filePath.Trim();
			//}
			//else
			//   filePath = path;

			//if (lix >= 0 && path.Length > lix + 1)
			//{
			//   args = path.Substring(lix + 1, path.Length - lix - 1);
			//   args = args.Trim();
			//}

			//return filePath;
		}

		public static bool FileExists(string path)
		{
			if (File.Exists(path))
				return true;

			var dir = ResolveFilePath(path);
			if (string.IsNullOrEmpty(dir))
				return false;

			return File.Exists(Path.Combine(dir, path));
		}

		public static string GetNextPathLevel(string path, string begin)
		{
			if (String.IsNullOrEmpty(path))
				return path;

			if (String.IsNullOrEmpty(begin))
				return Path.GetPathRoot(path);

			if (!path.StartsWith(begin, StringComparison.InvariantCultureIgnoreCase))
				return String.Empty;

			int ix = path.IndexOf(Path.DirectorySeparatorChar, begin.Length - 1);
			if (ix < 0)
				return String.Empty;

			int ixe = ix >= path.Length - 1 ? 
				-1 : path.IndexOf(Path.DirectorySeparatorChar, ix + 1);

			if (ixe < 0)
				return path.Substring(ix + 1, path.Length - ix - 1);
			else
				return path.Substring(ix + 1, ixe - ix - 1);
		}

		public static string GetUpperPath(string path)
		{
			if (String.IsNullOrEmpty(path))
				return path;

			int ix = path.LastIndexOf(Path.DirectorySeparatorChar);
			if (ix < 0)
				return path;

			ix = ix >= path.Length - 1 ?
				path.LastIndexOf(Path.DirectorySeparatorChar, path.Length - 2, path.Length - 1) : ix;

			if (ix < 0)
				return String.Empty;

			return path.Substring(0, ix);
		}

		public static bool IsAbsolutePath(string path)
		{
			var invalidChars = Path.GetInvalidPathChars();
			for (int i = 0; i < invalidChars.Length; i++)
			{
				if (path.IndexOf(invalidChars[i]) >= 0)
					return false;
			}

			if (!Path.IsPathRooted(path))
				return false;

			return true;
		}

		public static bool IsPathUNC(string path)
		{
			Uri uri;
			try
			{
				uri = new Uri(path);
			}
			catch (UriFormatException)
			{
				try
				{
					path = Path.GetFullPath(path);
					uri = new Uri(path);
				}
				catch 
				{
					return false;
				}
			}

			return uri.IsUnc;
		}

		public static bool IsLikeDrive(string path)
		{
			if (String.IsNullOrEmpty(path))
				return false;

			return
				Char.IsLetter(path[0]) && 
				path.Length == 3 && 
				path.EndsWith(":\\");
		}

		public static string ConcatUri(string uri1, string uri2)
		{
			if (String.IsNullOrEmpty(uri1))
				return uri2;

			if (String.IsNullOrEmpty(uri2))
				return uri1;

			StringBuilder buffer = new StringBuilder(uri1.Length + uri2.Length + 10);

			char delim = GetPathSeparator(uri1);

			buffer.Append(uri1);

			if (uri1[uri1.Length - 1] == delim &&
				 uri2[0] == delim)
				return buffer.Append(uri2, 1, uri2.Length - 1).ToString();

			if (uri1[uri1.Length - 1] != delim &&
				uri2[0] != delim)
			{
				buffer.Append(delim);
			}

			buffer.Append(uri2);
			return buffer.ToString();
		}

		public static char GetPathSeparator(string uri1)
		{
			if (String.IsNullOrEmpty(uri1))
				return Path.DirectorySeparatorChar;

			Uri u1 = new Uri(uri1);
			return u1.Scheme == Uri.UriSchemeFile ? Path.DirectorySeparatorChar : '/';
		}


		private static bool HasInvalidChars(string path)
		{
			char[] inv = Path.GetInvalidPathChars();
			for (int i = 0; i < inv.Length; i++)
			{
				if (path.IndexOf(inv[i]) >= 0)
					return true;
			}

			return false;
		}
	}
}

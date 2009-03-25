﻿using System;
using System.IO;


namespace CommonLib
{
	public static class PathHelper
	{
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

			while (true)
			{
				int lix = tempPath.LastIndexOf(' ');
				if (lix < 0)
					break;

				tempPath = tempPath.Substring(0, lix);
				tempPath = tempPath.Trim('\"');

				if (!tempPath.Contains("\"") && (File.Exists(tempPath) || Directory.Exists(tempPath)))
					break;
			}

			string argsTemp = path.Trim(' ', '\"');
			argsTemp = argsTemp.Substring(tempPath.Length, argsTemp.Length - tempPath.Length);
			argsTemp = argsTemp.Trim(' ', '\"');
			args = argsTemp;

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

			int ixe = ix >= path.Length - 1 ?
				path.LastIndexOf(Path.DirectorySeparatorChar, 0, path.Length - 1) : ix;

			return path.Substring(0, ix);
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
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace AppManager.Common
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

			if (File.Exists(tempPath) || Directory.Exists(tempPath))
				return tempPath;

			while (true)
			{
				int lix = tempPath.LastIndexOf(' ');
				if (lix < 0)
					break;

				tempPath = tempPath.Substring(0, lix);
				tempPath = tempPath.Trim('\"');

				if (File.Exists(tempPath) || Directory.Exists(tempPath))
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
	}
}

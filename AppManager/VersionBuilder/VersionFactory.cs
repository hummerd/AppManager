using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommonLib;
using CommonLib.IO;
using UpdateLib.VersionInfo;


namespace VersionBuilder
{
	public class VersionFactory
	{
		public void CreateVersion(string dir, Version version, string location, string excludeExt, string locales)
		{
			if (!Directory.Exists(dir))
				throw new Exception("Source dir " + dir + " does not exist");

			if (dir.EndsWith("\\"))
				dir = dir.Substring(0, dir.Length - 1);

			string versionDir = dir + "_" + version;

			if (Directory.Exists(versionDir))
				Directory.Delete(versionDir, true);

			var files = GetVersionFiles(dir, excludeExt);
			var verManifest = new VersionManifest() 
				{ VersionNumber = version, UpdateUri = location, UpdateUriAlt = location };

			char delim = PathHelper.GetPathSeparator(location);

			foreach (var item in files)
			{
				string newPath = item.Replace(dir, versionDir) + ".gzip";
				string newDir = Path.GetDirectoryName(newPath);
				if (!Directory.Exists(newDir))
					Directory.CreateDirectory(newDir);

				string itemPath;
				if (newDir.Length <= versionDir.Length)
					itemPath = String.Empty;
				else
				{
					itemPath = newDir.Substring(versionDir.Length + 1, newDir.Length - versionDir.Length - 1);

					if (String.IsNullOrEmpty(itemPath))
						itemPath = String.Empty;
				}

				//For assemblies version in manifest is assembly version
				//for another files version is system version
				AssemblyName an;
				try
				{
					an = AssemblyName.GetAssemblyName(item);
				}
				catch
				{
					an = null;
				}
				
				var ver = new Version(
					version.Major, 
					version.Minor, 
					version.Build, 
					(an == null ? version : an.Version).Revision);

				verManifest.VersionItems.Add(new VersionItem()
					{
						InstallAction = InstallAction.Copy,
						Location = location + newPath.Replace(versionDir, String.Empty).Replace('\\', delim),
						Path = itemPath,
						VersionNumber = ver,
						Base64Hash = FileHash.GetBase64FileHash(item)
					});
				
				GZipCompression.CompressFile(item, newPath);
			}

			XmlSerializeHelper.SerializeItem(
				verManifest, 
				Path.Combine(versionDir, VersionManifest.VersionManifestFileName));

			string[] locs;
			if (String.IsNullOrEmpty(locales))
				locs = new string[] { "en" };
			else
				locs = locales.Split(',');


			foreach (var item in locs)
			{
				XmlSerializeHelper.SerializeItem(
					new VersionData(version),
					Path.Combine(versionDir, String.Format(VersionManifest.VersionFileName, item))
					);
			}
		}


		protected List<string> GetVersionFiles(string dir, string excludeExt)
		{
			var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
			var result = new List<string>(files.Length);

			foreach (var item in files)
			{
				if (String.Equals(
						Path.GetFileName(item),
						VersionManifest.VersionManifestFileName,
						StringComparison.InvariantCultureIgnoreCase))
					continue;

				string ext = Path.GetExtension(item);
				ext = ext.Substring(1, ext.Length - 1);

				if (excludeExt.IndexOf(ext, StringComparison.CurrentCultureIgnoreCase) >= 0)
					continue;

				result.Add(item);
			}

			return result;
		}
	}
}

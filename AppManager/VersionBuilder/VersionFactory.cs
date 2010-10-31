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
		public void CreateVersion(string dir, Version version, string location, string excludeExt, string locales, string updaterSrcDir)
		{
			if (!Directory.Exists(dir))
				throw new Exception("Source dir " + dir + " does not exist");

			if (dir.EndsWith("\\"))
				dir = dir.Substring(0, dir.Length - 1);

			string versionDir = dir + "_" + version;
			string updaterDir = dir + "_" + version + "_Updater";

			if (Directory.Exists(versionDir))
				Directory.Delete(versionDir, true);

			var files = GetVersionFiles(dir, excludeExt);
			var verManifest = new VersionManifest() 
				{ VersionNumber = version, UpdateUri = location, UpdateUriAlt = location };

			char delim = PathHelper.GetPathSeparator(location);

			foreach (var item in files)
			{
				string newPath;
				string itemPath;
				CompressFile(item, dir, versionDir, out newPath, out itemPath);

				var ver = CreateVersion(version, item);

				verManifest.VersionItems.Add(new VersionItem()
					{
						InstallAction = InstallAction.Copy,
						Location = location + newPath.Replace(versionDir, String.Empty).Replace('\\', delim),
						Path = itemPath,
						VersionNumber = ver,
						Base64Hash = FileHash.GetBase64FileHash(item)
					});
			}

			files = GetVersionFiles(updaterSrcDir, excludeExt);

			foreach (var item in files)
			{
				string newPath;
				string itemPath;
				CompressFile(item, updaterSrcDir, updaterDir, out newPath, out itemPath);

				verManifest.BootStrapper.Add(new LocationHash()
				{
					Location = location + newPath.Replace(updaterDir, String.Empty).Replace('\\', delim),
					Path = itemPath,
					Base64Hash = FileHash.GetBase64FileHash(item)
				});
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


		protected void CompressFile(string path, string srcDir, string dstDir, out string newPath, out string itemPath)
		{
			newPath = path.Replace(srcDir, dstDir) + ".gzip";
			string newDir = Path.GetDirectoryName(newPath);
			if (!Directory.Exists(newDir))
				Directory.CreateDirectory(newDir);

			if (newDir.Length <= dstDir.Length)
				itemPath = String.Empty;
			else
			{
				itemPath = newDir.Substring(dstDir.Length + 1, newDir.Length - dstDir.Length - 1);

				if (String.IsNullOrEmpty(itemPath))
					itemPath = String.Empty;
			}

			GZipCompression.CompressFile(path, newPath);
		}

		protected Version CreateVersion(Version baseVersion, string path)
		{
			//For assemblies version in manifest is assembly version
			//for another files version is system version
			AssemblyName an;
			try
			{
				an = AssemblyName.GetAssemblyName(path);
			}
			catch
			{
				an = null;
			}

			return new Version(
				baseVersion.Major,
				baseVersion.Minor,
				baseVersion.Build,
				(an == null ? baseVersion : an.Version).Revision);
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

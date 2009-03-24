﻿using System;
using System.IO;
using System.IO.Compression;
using CommonLib;
using UpdateLib.Install;
using UpdateLib.VersionNumberProvider;
using System.Reflection;
using CommonLib.IO;


namespace VersionBuilder
{
	public class VersionFactory
	{
		public void CreateVersion(string dir, Version version, string location)
		{
			if (!Directory.Exists(dir))
				return;

			string versionDir = dir + "_" + version;

			if (Directory.Exists(versionDir))
				Directory.Delete(versionDir, true);
			
			var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
			var verManifest = new VersionManifest() 
				{ VersionNumber = version};

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

				verManifest.VersionItems.Add(new VersionItem()
					{
						InstallAction = InstallAction.Copy,
						Location = location + newPath.Replace(versionDir, String.Empty),
						Path = itemPath,
						VersionNumber = an == null ? version : an.Version
					});
				
				GZipCompression.CompressFile(item, newPath);
			}

			XmlSerializeHelper.SerializeItem(
				verManifest, 
				Path.Combine(versionDir, VersionManifest.VersionManifestFileName));

			XmlSerializeHelper.SerializeItem(
				new VersionInfo(version),
				Path.Combine(versionDir, VersionManifest.VersionFileName));
		}
		


	}
}

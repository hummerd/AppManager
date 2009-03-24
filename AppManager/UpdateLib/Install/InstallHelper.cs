using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionNumberProvider;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using CommonLib.IO;
using CommonLib;


namespace UpdateLib.Install
{
	public class InstallHelper
	{
		public InstallHelper()
		{

		}


		public void InstallVersion(string tempPath, VersionManifest downloadedManifest, VersionManifest latestManifest)
		{
			//Unzip
			foreach (var item in downloadedManifest.VersionItems)
			{
				var tempFile = Path.Combine(tempPath, item.GetItemFullPath());
				var tempFileUnzip = tempFile.Substring(0, tempFile.Length - Path.GetExtension(tempFile).Length);

				GZipCompression.DecompressFile(tempFile, tempFileUnzip);
			}

			//Saving latest manifest
			XmlSerializeHelper.SerializeItem(
				latestManifest,
				Path.Combine(tempPath, VersionManifest.VersionManifestFileName));

			//Saving downloaded manifest
			XmlSerializeHelper.SerializeItem(
				downloadedManifest,
				Path.Combine(tempPath, VersionManifest.DownloadedVersionManifestFileName));

			string installerPath = Path.Combine(tempPath, "Updater.exe");
			File.WriteAllBytes(installerPath, Resource.Updater);

			Process.Start(installerPath, "-pn" + Process.GetCurrentProcess().ProcessName);
		}
	}
}

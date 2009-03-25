using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using CommonLib;
using CommonLib.IO;
using CommonLib.Windows;
using UpdateLib;
using UpdateLib.VersionInfo;


namespace Updater
{
	public class MainWorkItem
	{
		public MainWorkItem()
		{

		}


		public void Run(string[] args)
		{
			try
			{
#if DEBUG
				MessageBox.Show(null, "AttachDebug", "");
#endif

				string installerPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				var installInfo = XmlSerializeHelper.DeserializeItem(
					typeof(InstallInfo),
					Path.Combine(installerPath, InstallInfo.InstallInfoFileName)
					) as InstallInfo;

				var downloadManifest = XmlSerializeHelper.DeserializeItem(
					typeof(VersionManifest),
					Path.Combine(installInfo.TempPath, VersionManifest.DownloadedVersionManifestFileName)
					) as VersionManifest;

				if (!CheckHash(installInfo.TempPath, downloadManifest.VersionItems))
				{
					ErrorBox.Show("Check sum error", "Download version once again", "");
					return;
				}
				
				WaitForProccesses(installInfo.LockProcess);
				InstallUpdate(installInfo.TempPath, installInfo.InstallPath, downloadManifest.VersionItems);
				RunExecutables(installInfo.InstallPath, installInfo.ExecutePaths, downloadManifest.VersionItems);
				CleanUp(installInfo.TempPath);
			}
			catch (Exception exc)
			{
				ErrorBox.Show("Update error", exc);
			}
		}


		protected bool CheckHash(string tempPath, VersionItemList versionItems)
		{
			foreach (var item in versionItems)
			{
				if (FileHash.GetBase64FileHash(Path.Combine(tempPath, item.GetUnzipItemFullPath())) != 
					item.Base64Hash)
				return false;
			}

			return true;
		}

		protected void WaitForProccesses(string[] lockProcesses)
		{
			bool freeToGo;

			//wating for 
			do
			{
				freeToGo = true;

				foreach (var item in lockProcesses)
					freeToGo = freeToGo && Process.GetProcessesByName(item).Length <= 0;

				//Application.DoEvents();
				Thread.Sleep(1000);

			} while (!freeToGo);
		}

		protected void InstallUpdate(string tempPath, string appPath, VersionItemList versionItems)
		{
			//first copy new version manifest
			File.Copy(
				Path.Combine(tempPath, VersionManifest.VersionManifestFileName),
				Path.Combine(appPath, VersionManifest.VersionManifestFileName),
				true);

			//copy new files
			foreach (var item in versionItems)
			{
				if (item.NeedCopyItem())
					File.Copy(
						Path.Combine(tempPath, item.GetUnzipItemFullPath()),
						Path.Combine(appPath, item.GetUnzipItemFullPath()),
						true);
			}
		}

		protected void RunExecutables(string appPath, string[] execs, VersionItemList versionItems)
		{
			foreach (var item in execs)
				Process.Start(item);

			foreach (var item in versionItems)
			{
				if (!item.NeedRunItem())
					continue;

				var itemPath = Path.Combine(appPath, item.GetUnzipItemFullPath());
				bool exists = Array.Exists(execs, 
					str => String.Equals(str, itemPath, StringComparison.InvariantCultureIgnoreCase));

				if (!exists)
					Process.Start(itemPath);
			}
		}

		protected void CleanUp(string tempPath)
		{
			Directory.Delete(tempPath, true);
		}
	}
}

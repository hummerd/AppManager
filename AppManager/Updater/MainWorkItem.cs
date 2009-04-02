using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using CommonLib;
using CommonLib.IO;
using CommonLib.Windows;
using UpdateLib;
using UpdateLib.VersionInfo;


namespace Updater
{
	public class MainWorkItem
	{
		protected Mutex _UpdatingFlag;


		public MainWorkItem()
		{

		}


		public void Run()
		{
			try
			{
#if DEBUG
				CommonLib.Windows.MsgBox.Show(null, "AttachDebug", "");
#endif
			
				string installerPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				var installInfo = XmlSerializeHelper.DeserializeItem(
					typeof(InstallInfo),
					Path.Combine(installerPath, InstallInfo.InstallInfoFileName)
					) as InstallInfo;

				bool notAlredyRunning;
				_UpdatingFlag = new Mutex(true, InstallInfo.InstallInfoMutexPrefix + installInfo.AppName, out notAlredyRunning);
				if (!notAlredyRunning)
				{
					//RunExecutables(installInfo.InstallPath, installInfo.ExecutePaths, null);
					return;
				}
				//_UpdatingFlag.WaitOne();

				var downloadManifest = XmlSerializeHelper.DeserializeItem(
					typeof(VersionManifest),
					Path.Combine(installInfo.TempPath, VersionManifest.DownloadedVersionManifestFileName)
					) as VersionManifest;

				if (!CheckHash(installInfo.TempPath, downloadManifest.VersionItems))
				{
					ErrorBox.Show(UIStrings.Str("UPDATER"), UIStrings.Str("BAD_VERSION"), String.Empty);
					RunExecutables(installInfo.InstallPath, installInfo.ExecutePaths, null);
					CleanUp(installInfo.TempPath);
					return;
				}
				
				WaitForProccesses(installInfo.LockProcess);
				InstallUpdate(installInfo.TempPath, installInfo.InstallPath, downloadManifest.VersionItems);
				RunExecutables(installInfo.InstallPath, installInfo.ExecutePaths, downloadManifest.VersionItems);
				CleanUp(installInfo.TempPath);
			}
			catch (Exception exc)
			{
				ErrorBox.Show(UIStrings.Str("UPDATER"), exc);
			}

			if (_UpdatingFlag != null)
			{
				//_UpdatingFlag.ReleaseMutex();
				_UpdatingFlag.Close();
			}

			Application.Current.Shutdown();
		}


		protected bool CheckHash(string tempPath, VersionItemList versionItems)
		{
			foreach (var item in versionItems)
			{
				if (item.InstallAction == InstallAction.Delete)
					continue;

				if (FileHash.GetBase64FileHash(Path.Combine(tempPath, item.GetUnzipItemFullPath())) != 
					item.Base64Hash)
				return false;
			}

			return true;
		}

		protected bool WaitForProccesses(string[] lockProcesses)
		{
			bool freeToGo = true;
			foreach (var item in lockProcesses)
				freeToGo = freeToGo && Process.GetProcessesByName(item).Length <= 0;

			if (freeToGo)
				return true;

			//wating for 
			var startWait = DateTime.Now;

			do
			{
				freeToGo = true;

				foreach (var item in lockProcesses)
					freeToGo = freeToGo && Process.GetProcessesByName(item).Length <= 0;

				Thread.Sleep(1000);

				if (!freeToGo && (DateTime.Now - startWait).TotalSeconds > 5)
				{
					var prs = GetWaitProcccesses(lockProcesses);
					WaitForProcces wait = new WaitForProcces(prs);
					var dr = wait.ShowDialog();
					if (!(dr ?? false))
						return false;
				}

			} while (!freeToGo);

			return true;
		}

		protected List<string> GetWaitProcccesses(string[] lockProcesses)
		{
			var result = new List<string>();

			foreach (var item in lockProcesses)
			{
				var prs = Process.GetProcessesByName(item);
				var pns = Array.ConvertAll(prs, p => p.ProcessName + " - " + p.MainWindowTitle);
				result.AddRange(pns);
			}

			return result;
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

				if (item.InstallAction == InstallAction.Delete)
				{
					var path = Path.Combine(appPath, item.GetUnzipItemFullPath());
					if (File.Exists(path))
						File.Delete(path);
					else if (Directory.Exists(path))
						Directory.Delete(path, true);
				}
			}
		}

		protected void RunExecutables(string appPath, string[] execs, VersionItemList versionItems)
		{
			foreach (var item in execs)
				Process.Start(item);

			if (versionItems == null)
				return;

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

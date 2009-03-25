using System;
using System.IO;
using UpdateLib.FileDownloader;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;
using UpdateLib.ShareUpdate;
using System.Threading;
using System.Diagnostics;
using CommonLib.IO;
using CommonLib;
using System.Windows;


namespace UpdateLib
{
	public class SelfUpdate
	{
		public static SelfUpdate CreateShareUpdate()
		{
			var updater = new SelfUpdate()
			{
				FileDownloader = new ShareFileDownloader(),
				VersionNumberProvider = new ShareVNP(),
				UIAskDownload = new AskDownload(),
				UIAskInstall = new AskDownload(),
				UIDownloadProgress = new DownloadProgress()
			};

			return updater;
		}


		protected Mutex _UpdatingFlag;
		protected FileDownloadHelper _Downloader;


		public SelfUpdate()
		{

		}


		public IFileDownloader FileDownloader
		{ get; set; }

		public IVersionNumberProvider VersionNumberProvider
		{ get; set; }

		public IUIAskDownload UIAskDownload
		{ get; set; }

		public IUIAskInstall UIAskInstall
		{ get; set; }

		public IUIDownloadProgress UIDownloadProgress
		{ get; set; }


		//public void UpdateApp()
		//{
		//   Version currentVersion = null;
		//   UpdateApp(currentVersion, String.Empty, String.Empty);
		//}

		public bool UpdateApp(
			VersionManifest currentManifest, 
			string newVersionLocation, 
			string appName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses)
		{
			bool notAlredyRunning;
			_UpdatingFlag = new Mutex(true, "Global\\UpdateLib_" + appName, out notAlredyRunning);
			if (!notAlredyRunning)
				return false;

			try
			{
				Version lastVersion = GetLatestVersion(newVersionLocation);
				if (lastVersion > currentManifest.VersionNumber)
				{
					_Downloader = new FileDownloadHelper(FileDownloader, UIDownloadProgress);

					VersionInfo latestVersionInfo = VersionNumberProvider.GetLatestVersionInfo(newVersionLocation);
					if (latestVersionInfo == null)
					{
						_UpdatingFlag.ReleaseMutex();
						return false;
					}

					if (AskUserForDownload(latestVersionInfo))
					{
						VersionManifest latestManifest = VersionNumberProvider.GetLatestVersionManifest(newVersionLocation);
						if (latestVersionInfo == null)
						{
							_UpdatingFlag.ReleaseMutex();
							return false;
						}

						string tempPath = CreateTempDir(appName, latestVersionInfo);

						VersionManifest updateManifest = latestManifest.GetUpdateManifest(currentManifest);

						_Downloader.DownloadCompleted += (s, e) => OnVersionDownloadCompleted(e);
						_Downloader.DownloadVersion(
							new VersionDownloadInfo()
							{
								AppName = appName,
								AppPath = appPath,
								ExecutePaths = executePaths,
								LockProcesses = lockProcesses,
								DownloadedVersionManifest = updateManifest,
								LatestVersionManifest = latestManifest,
								LatestVersionInfo = latestVersionInfo,
								CurrentVersionManifest = currentManifest,
								TempPath = tempPath
							});
					}
				}
			}
			catch
			{
				_UpdatingFlag.ReleaseMutex();
				return false;
			}

			return true;
		}


		protected Version GetLatestVersion(string location)
		{
			return VersionNumberProvider.GetLatestVersionInfo(location).VersionNumber;
		}

		protected bool AskUserForDownload(VersionInfo versionInfo)
		{
			return UIAskDownload.AskForDownload(versionInfo);
		}

		protected string CreateTempDir(string appName, VersionInfo latestVersionInfo)
		{
			string path = Path.GetTempPath();
			path = Path.Combine(path, "UpdateLib");
			path = Path.Combine(path, appName + "_" + latestVersionInfo);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			Directory.CreateDirectory(path);

			return path;
		}

		protected bool AskUserForInstall(VersionInfo versionInfo)
		{
			return UIAskInstall.AskForInstall(versionInfo);
		}

		protected void OnVersionDownloadCompleted(VersionDownloadInfo versionDownLoad)
		{
			if (versionDownLoad.Succeded && AskUserForInstall(versionDownLoad.LatestVersionInfo))
			{
				//Unzip
				foreach (var item in versionDownLoad.DownloadedVersionManifest.VersionItems)
				{
					var tempFile = Path.Combine(versionDownLoad.TempPath, item.GetItemFullPath());
					var tempFileUnzip = tempFile.Substring(0, tempFile.Length - Path.GetExtension(tempFile).Length);

					GZipCompression.DecompressFile(tempFile, tempFileUnzip);
				}

				//Saving latest manifest
				XmlSerializeHelper.SerializeItem(
					versionDownLoad.LatestVersionManifest,
					Path.Combine(versionDownLoad.TempPath, VersionManifest.VersionManifestFileName));

				//Saving downloaded manifest
				XmlSerializeHelper.SerializeItem(
					versionDownLoad.DownloadedVersionManifest,
					Path.Combine(versionDownLoad.TempPath, VersionManifest.DownloadedVersionManifestFileName));

				//Saving install info
				XmlSerializeHelper.SerializeItem(
					new InstallInfo(versionDownLoad),
					Path.Combine(versionDownLoad.TempPath, InstallInfo.InstallInfoFileName));

				string installerPath = Path.Combine(versionDownLoad.TempPath, "Updater.exe");
				File.WriteAllBytes(installerPath, Resource.Updater);
				
				//Start installing
				Process.Start(installerPath);
				_UpdatingFlag.ReleaseMutex();
				Application.Current.Shutdown();
			}

			_UpdatingFlag.ReleaseMutex();
		}
	}

	[Serializable]
	public class InstallInfo
	{
		public const string InstallInfoFileName = "InstallInfo.xml";


		public InstallInfo()
		{

		}

		public InstallInfo(VersionDownloadInfo versionInfo)
		{
			LockProcess = versionInfo.LockProcesses;
			InstallPath = versionInfo.AppPath;
			ExecutePaths = versionInfo.ExecutePaths;
		}

		public string[] LockProcess
		{ get; set; }
		public string InstallPath
		{ get; set; }
		public string[] ExecutePaths
		{ get; set; }
	}
}

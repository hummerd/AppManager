using System;
using System.IO;
using UpdateLib.FileDownloader;
using UpdateLib.Install;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;
using UpdateLib.ShareUpdate;
using System.Threading;


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
		protected InstallHelper _Installer;


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

		public bool UpdateApp(Version currentVersion, VersionManifest currentManifest, string location, string appName)
		{
			bool notAlredyRunning;
			_UpdatingFlag = new Mutex(true, "Global\\UpdateLib_" + appName, out alredyRunning);
			if (!notAlredyRunning)
				return false;

			try
			{
				Version lastVersion = GetLatestVersion(location);
				if (lastVersion > currentVersion)
				{
					_Downloader = new FileDownloadHelper(FileDownloader, UIDownloadProgress);

					VersionInfo versionInfo = VersionNumberProvider.GetLatestVersionInfo(location);
					if (versionInfo == null)
					{
						_UpdatingFlag.ReleaseMutex();
						return false;
					}

					if (AskUserForDownload(versionInfo))
					{
						VersionManifest verManifest = VersionNumberProvider.GetLatestVersionManifest(location);
						if (versionInfo == null)
						{
							_UpdatingFlag.ReleaseMutex();
							return false;
						}

						string tempPath = CreateTempDir(appName, versionInfo);

						VersionManifest updateManifest = verManifest.GetUpdateManifest(currentManifest);

						_Downloader.DownloadCompleted += (s, e) => OnVersionDownloadCompleted(
							e.Succeded, 
							e.DownloadedVersionManifest, 
							e.DownloadedVersionInfo, 
							e.LatestVersion, 
							e.AppName, 
							e.TempPath);

						_Downloader.DownloadVersion(updateManifest, versionInfo, lastVersion, appName, tempPath);
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
			if (versionDownLoad.Succeded && AskUserForInstall(versionDownLoad.DownloadedVersionInfo))
				_Installer.InstallVersion(
					versionDownLoad.TempPath, 
					versionDownLoad.DownloadedVersionManifest,
					versionDownLoad.LatestVersionManifest
					);

			_UpdatingFlag.ReleaseMutex();
		}
				
	}
}

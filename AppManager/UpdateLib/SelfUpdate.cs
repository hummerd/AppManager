using System;
using System.IO;
using UpdateLib.FileDownloader;
using UpdateLib.Install;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;
using UpdateLib.ShareUpdate;


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

		public void UpdateApp(Version currentVersion, string location, string appName)
		{
			_Downloader = new FileDownloadHelper(FileDownloader, UIDownloadProgress);

			Version lastVersion = GetLatestVersion(location);
			if (lastVersion > currentVersion)
			{
				VersionInfo versionInfo = VersionNumberProvider.GetLatestVersionInfo(location);
				if (AskUserForDownload(versionInfo))
				{
					VersionManifest verManifest = VersionNumberProvider.GetLatestVersionManifest(location);
					string tempPath = CreateTempDir();

					_Downloader.DownloadCompleted += (s, e) => OnVersionDownloadCompleted(
						e.Succeded, e.DownloadedVersionManifest, e.DownloadedVersionInfo, e.LatestVersion, e.AppName, e.TempPath);
					_Downloader.DownloadVersion(verManifest, versionInfo, lastVersion, appName, tempPath);
				}
			}
		}


		protected void OnVersionDownloadCompleted(
			bool succeded, 
			VersionManifest manifest, 
			VersionInfo versionInfo, 
			Version latestVersion, 
			string appName, 
			string tempPath)
		{
			if (succeded && AskUserForInstall(versionInfo))
				_Installer.InstallVersion(tempPath, manifest, latestVersion);
		}
				
		protected Version GetLatestVersion(string location)
		{
			return VersionNumberProvider.GetLatestVersionInfo(location).VersionNumber;
		}

		protected bool AskUserForDownload(VersionInfo versionInfo)
		{
			return UIAskDownload.AskForDownload(versionInfo);
		}

		protected string CreateTempDir()
		{
			string path =  Path.GetTempPath();
			path = Path.Combine(path, "UpdateLib");
			string currentTemp = DateTime.Now.Ticks.ToString().Substring(0, 10);
			path = Path.Combine(path, currentTemp);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			Directory.CreateDirectory(path);

			return path;
		}

		protected bool AskUserForInstall(VersionInfo versionInfo)
		{
			return UIAskInstall.AskForInstall(versionInfo);
		}
	}
}

using System;
using System.IO;
using UpdateLib.FileDownloader;
using UpdateLib.UI;
using UpdateLib.VersionNumberProvider;
using UpdateLib.Install;


namespace UpdateLib
{
	public class SelfUpdate
	{
		protected FileDownloadHelper _Downloader;
		protected InstallHelper _Installer;


		public SelfUpdate()
		{

		}


		public IFileDownloader FileDownloader { get; set; }

		public IVersionNumberProvider VersionNumberProvider { get; set; }

		public IUIAskDownload UIAskDownload { get; set; }


		public void UpdateApp()
		{
			Version currentVersion = null;
			UpdateApp(currentVersion);
		}

		public void UpdateApp(Version currentVersion)
		{
			_Downloader = new FileDownloadHelper(FileDownloader);

			Version lastVersion = GetLatestVersion();
			if (lastVersion > currentVersion)
			{ 
				VersionInfo versionInfo = VersionNumberProvider.GetLatestVersionInfo();
				if (AskUserForDownload(versionInfo, lastVersion))
				{
					VersionManifest verManifest = VersionNumberProvider.GetLatestVersionManifest();
					string tempPath = CreateTempDir();

					_Downloader.DownloadVersion(verManifest, lastVersion);

					if (AskUserForInstall(versionInfo, lastVersion))
						_Installer.InstallVersion(verManifest, lastVersion);
				}
			}
		}

		
		protected Version GetLatestVersion()
		{
			return VersionNumberProvider.GetLatestVersion();
		}

		protected bool AskUserForDownload(VersionInfo versionInfo, Version version)
		{
			return UIAskDownload.AskDownload(versionInfo, version);
		}

		protected string CreateTempDir()
		{
			string path =  Path.GetTempPath();
			path = Path.Combine(path, "UpdateLib");
			string currentTemp = DateTime.Now.Ticks.ToString().Substring(0, 10);
			path = Path.Combine(path, currentTemp);

			if (Directory.Exists(path))
				Directory.Delete(path);

			return path;
		}

		protected bool AskUserForInstall(VersionInfo versionInfo, Version version)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using CommonLib;
using CommonLib.IO;
using UpdateLib.FileDownloader;
using UpdateLib.ShareUpdate;
using UpdateLib.UI;
using UpdateLib.VersionInfo;
using System.Resources;
using System.Globalization;


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
			string newVersionLocation,
			string appName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses)
		{
			var currentManifest = XmlSerializeHelper.DeserializeItem(
				typeof(VersionManifest),
				Path.Combine(appPath, VersionManifest.VersionManifestFileName)
				) as VersionManifest;

			return UpdateApp(
				currentManifest,
				newVersionLocation,
				appName,
				appPath,
				executePaths,
				lockProcesses
				);
		}

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
				CleanUp(appName, currentManifest);

				Version lastVersion = GetLatestVersion(newVersionLocation);
				if (lastVersion > currentManifest.VersionNumber)
				{
					_Downloader = new FileDownloadHelper(FileDownloader, UIDownloadProgress);

					VersionData latestVersionInfo = VersionNumberProvider.GetLatestVersionInfo(newVersionLocation);
					if (latestVersionInfo == null)
					{
						_UpdatingFlag.Close();
						return false;
					}

					if (AskUserForDownload(latestVersionInfo))
					{
						VersionManifest latestManifest = VersionNumberProvider.GetLatestVersionManifest(newVersionLocation);
						if (latestVersionInfo == null)
						{
							_UpdatingFlag.Close();
							return false;
						}

						string tempPath = CreateTempDir(appName, latestManifest.VersionNumber);

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
				_UpdatingFlag.Close();
				return false;
			}

			return true;
		}


		protected void CleanUp(string appName, VersionManifest currentManifest)
		{
			var tempPath = GetTempDir(appName, currentManifest.VersionNumber);
			var instDir = GetInstallerDir(tempPath, appName, currentManifest.VersionNumber);

			if (Directory.Exists(instDir))
				Directory.Delete(instDir, true);
		}

		protected Version GetLatestVersion(string location)
		{
			return VersionNumberProvider.GetLatestVersionInfo(location).VersionNumber;
		}

		protected bool AskUserForDownload(VersionData versionInfo)
		{
			return UIAskDownload.AskForDownload(versionInfo);
		}

		protected string GetTempDir(string appName, Version latestVersion)
		{
			string path = Path.GetTempPath();
			path = Path.Combine(path, "UpdateLib");
			path = Path.Combine(path, appName + "_" + latestVersion);

			return path;
		}

		protected string CreateTempDir(string appName, Version latestVersion)
		{
			string path = GetTempDir(appName, latestVersion);

			if (Directory.Exists(path))
				Directory.Delete(path, true);

			Directory.CreateDirectory(path);

			return path;
		}

		protected string GetInstallerDir(string tempPath, string appName, Version latestVersion)
		{
			var installerDir = PathHelper.GetUpperPath(tempPath);
			installerDir = Path.Combine(installerDir,
				appName + "_" + latestVersion + "_Inst");

			return installerDir;
		}

		protected bool AskUserForInstall(VersionData versionInfo)
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
					var tempFileUnzip = Path.Combine(versionDownLoad.TempPath, item.GetUnzipItemFullPath());

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

				var installerPath = CopyInstaller(versionDownLoad);
				
				//Start installing
				Process.Start(installerPath);
				_UpdatingFlag.Close();
				Application.Current.Shutdown();
			}

			_UpdatingFlag.Close();
		}

		protected string CopyInstaller(VersionDownloadInfo versionDownLoad)
		{
			var installerDir = GetInstallerDir(versionDownLoad.TempPath, versionDownLoad.AppName, versionDownLoad.LatestVersionManifest.VersionNumber);

			if (Directory.Exists(installerDir))
				Directory.Delete(installerDir, true);

			Directory.CreateDirectory(installerDir);

			string installerPath = Path.Combine(installerDir, "Updater.exe");
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream("UpdateLib.Resources.Updater.exe"))
			{
				BinaryReader br = new BinaryReader(stream);
				var updater = br.ReadBytes((int)br.BaseStream.Length);

				File.WriteAllBytes(installerPath, updater);
			}
			
			//Copy CommonLib
			string commonLibPath = Assembly.GetAssembly(typeof(XmlSerializeHelper)).Location;
			string temp = Path.Combine(installerDir, Path.GetFileName(commonLibPath));
			File.Copy(commonLibPath, temp, true);

			//Copy UpdateLib
			string updateLibPath = Assembly.GetExecutingAssembly().Location;
			temp = Path.Combine(installerDir, Path.GetFileName(updateLibPath));
			File.Copy(updateLibPath, temp, true);

			//Saving install info
			XmlSerializeHelper.SerializeItem(
				new InstallInfo(versionDownLoad),
				Path.Combine(installerDir, InstallInfo.InstallInfoFileName));

			return installerPath;
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
			TempPath = versionInfo.TempPath;
			ExecutePaths = versionInfo.ExecutePaths;
		}

		public string[] LockProcess
		{ get; set; }
		public string InstallPath
		{ get; set; }
		public string TempPath
		{ get; set; }
		public string[] ExecutePaths
		{ get; set; }
	}
}

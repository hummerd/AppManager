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
using System.Collections.Generic;
using CommonLib.Windows;


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
				UIAskDownload = new UIAsk(),
				UIAskInstall = new UIAsk(),
				UIDownloadProgress = new DownloadProgress()
			};

			return updater;
		}


		public event EventHandler NeedCloseApp;


		protected Mutex _UpdatingFlag;
		protected Mutex _InstUpdatingFlag;
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
			try
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
			catch(Exception exc)
			{
				ErrorBox.Show(UpdStr.UPDATER, exc);
			}

			return false;
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

			bool notInstAlredyRunning;
			_InstUpdatingFlag = new Mutex(true, InstallInfo.InstallInfoMutexPrefix + appName, out notInstAlredyRunning);
			//Installer alredy runninig
			if (!notInstAlredyRunning)
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

					if (!notInstAlredyRunning)
					{
						_UpdatingFlag.Close();
						return false;
					}
					
					if (AskUserForDownload(appName, latestVersionInfo))
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
			try
			{
				var tempPath = GetTempDir(appName, currentManifest.VersionNumber);
				var instDir = GetInstallerDir(tempPath, appName, currentManifest.VersionNumber);

				if (Directory.Exists(instDir))
					Directory.Delete(instDir, true);
			}
			catch
			{ ; }
		}

		protected Version GetLatestVersion(string location)
		{
			return VersionNumberProvider.GetLatestVersionInfo(location).VersionNumber;
		}

		protected bool AskUserForDownload(string appName, VersionData versionInfo)
		{
			return UIAskDownload.AskForDownload(appName, versionInfo);
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

		protected bool AskUserForInstall(string appName, VersionData versionInfo)
		{
			return UIAskInstall.AskForInstall(appName, versionInfo);
		}

		protected void OnVersionDownloadCompleted(VersionDownloadInfo versionDownLoad)
		{
			if (versionDownLoad.Succeded && AskUserForInstall(versionDownLoad.AppName, versionDownLoad.LatestVersionInfo))
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

				OnNeedCloseApp();
				//Application.Current.Shutdown();
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
			CopyAssembly(installerDir, Assembly.GetAssembly(typeof(XmlSerializeHelper)));
			//string commonLibPath = Assembly.GetAssembly(typeof(XmlSerializeHelper)).Location;
			//string temp = Path.Combine(installerDir, Path.GetFileName(commonLibPath));
			//File.Copy(commonLibPath, temp, true);

			//Copy UpdateLib
			CopyAssembly(installerDir, Assembly.GetExecutingAssembly());
			//string updateLibPath = Assembly.GetExecutingAssembly().Location;
			//temp = Path.Combine(installerDir, Path.GetFileName(updateLibPath));
			//File.Copy(updateLibPath, temp, true);

			//Saving install info
			XmlSerializeHelper.SerializeItem(
				new InstallInfo(versionDownLoad),
				Path.Combine(installerDir, InstallInfo.InstallInfoFileName));

			return installerPath;
		}

		protected void CopyAssembly(string destDir, Assembly asm)
		{
			string asmDir = Path.GetDirectoryName(asm.Location);
			File.Copy(asm.Location, asm.Location.Replace(asmDir, destDir), true);

			var sats = GetSateliteAsms(asm);
			foreach (var item in sats)
			{
				//asmDir = Path.GetDirectoryName(item.Location);
				var destPath = item.Location.Replace(asmDir, destDir);
				var destAsmDir = Path.GetDirectoryName(destPath);
				if (!Directory.Exists(destAsmDir))
					Directory.CreateDirectory(destAsmDir);

				File.Copy(item.Location, destPath, true);
			}
		}

		protected List<Assembly> GetSateliteAsms(Assembly asm)
		{
			var result = new List<Assembly>();

			try
			{
				var cults = CultureInfo.GetCultureInfo("ru");
				var sa = asm.GetSatelliteAssembly(cults);
				result.Add(sa);
			}
			catch
			{ ; }
			//foreach (var item in cults)
			//{
			//   var sa = asm.GetSatelliteAssembly(item);
			//   if (sa != null)
			//      result.Add(sa);
			//}

			return result;

		}
	
		protected virtual void OnNeedCloseApp()
		{
			if (NeedCloseApp != null)
				NeedCloseApp(this, EventArgs.Empty);
		}
	}

	[Serializable]
	public class InstallInfo
	{
		public const string InstallInfoFileName = "InstallInfo.xml";
		public const string InstallInfoMutexPrefix = "Global\\UpdateLibInst_";


		public InstallInfo()
		{

		}

		public InstallInfo(VersionDownloadInfo versionInfo)
		{
			LockProcess = versionInfo.LockProcesses;
			InstallPath = versionInfo.AppPath;
			TempPath = versionInfo.TempPath;
			ExecutePaths = versionInfo.ExecutePaths;
			AppName = versionInfo.AppName;
		}

		public string[] LockProcess
		{ get; set; }
		public string InstallPath
		{ get; set; }
		public string TempPath
		{ get; set; }
		public string AppName
		{ get; set; }
		public string[] ExecutePaths
		{ get; set; }
	}
}

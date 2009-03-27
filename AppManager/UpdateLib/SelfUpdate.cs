using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using CommonLib;
using CommonLib.Application;
using CommonLib.IO;
using CommonLib.Windows;
using UpdateLib.FileDownloader;
using UpdateLib.ShareUpdate;
using UpdateLib.UI;
using UpdateLib.VersionInfo;
using UpdateLib.WebUpdate;


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

		public static SelfUpdate CreateWebUpdate()
		{
			var updater = new SelfUpdate()
			{
				FileDownloader = new WebFileDownloader(),
				VersionNumberProvider = new WebVNP(),
				UIAskDownload = new UIAsk(),
				UIAskInstall = new UIAsk(),
				UIDownloadProgress = new DownloadProgress()
			};

			return updater;
		}


		public event EventHandler NeedCloseApp;


		protected delegate void SimpleMathod();
		protected delegate void UpdateDownloadProgress(string location, long total, long progress);
		protected delegate void SetDownloadProgressInfo(VersionManifest manifest);


		protected Mutex		_UpdatingFlag;
		protected Thread	_UpdateThread;


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

		public bool UpdateAppAsync(
			string appName,
			string displayAppName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses)
		{
			if (_UpdateThread != null && _UpdateThread.IsAlive)
				return false;

			_UpdateThread = new Thread(UpdateAppThread);
			_UpdateThread.SetApartmentState(ApartmentState.STA);
			_UpdateThread.IsBackground = true;
			_UpdateThread.Start(new object[]
				{
				appName,
				displayAppName,
				appPath,
				executePaths,
				lockProcesses
				});
			return true;
		}

		public bool UpdateApp(
			string appName,
			string displayAppName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses)
		{
			try
			{
				var manifestPath = Path.Combine(appPath, VersionManifest.VersionManifestFileName);
				VersionManifest currentManifest = null;

				if (File.Exists(manifestPath))
					currentManifest = XmlSerializeHelper.DeserializeItem(
						typeof(VersionManifest),
						manifestPath
						) as VersionManifest;

				return UpdateApp(
					currentManifest,
					appName,
					displayAppName,
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
			string appName,
			string displayAppName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses)
		{
			try
			{
				//if update alredy running quit
				if (!CheckOtherProccesses(appName))
					return false;

				//if there is now cuurent version info supose we have first version
				if (currentManifest == null)
					currentManifest = new VersionManifest() { VersionNumberString = "1.0.0.0", UpdateUri = "http://hummerd.com/AppManagerUpdate" };

				//clean up older install
				CleanUp(appName, currentManifest);

				var updateUri = currentManifest.UpdateUri;
				Version lastVersion = GetLatestVersion(updateUri);
				if (lastVersion == null)
				{
					updateUri = currentManifest.UpdateUriAlt;
					lastVersion = GetLatestVersion(updateUri);				
				}

				if (lastVersion > currentManifest.VersionNumber)
				{
					VersionData latestVersionInfo = VersionNumberProvider.GetLatestVersionInfo(updateUri);
					if (latestVersionInfo == null)
						return false;

					if (AskUserForDownload(displayAppName, latestVersionInfo, updateUri))
					{
						VersionManifest latestManifest = VersionNumberProvider.GetLatestVersionManifest(updateUri);
						//failed to download latest version manifest
						if (latestVersionInfo == null)
							return false;

						VersionManifest updateManifest = latestManifest.GetUpdateManifest(currentManifest);

						var tempPath = CreateTempDir(appName, latestManifest.VersionNumber);
						if (DownloadVersion(updateManifest, tempPath))
							VersionDownloadCompleted(
								displayAppName,
								updateManifest,
								latestManifest,
								latestVersionInfo,
								new InstallInfo() { 
									AppName = appName,
									ExecutePaths = executePaths,
									InstallPath = appPath,
									LockProcess = lockProcesses,
									TempPath = tempPath});
					}
				}
			}
			catch
			{
				return false;
			}
			finally
			{
				_UpdatingFlag.Close();
			}

			return true;
		}


		protected void UpdateAppThread(object prm)
		{
			var prms = prm as object[];

			UpdateApp(
				(string)prms[0],
				(string)prms[1],
				(string)prms[2],
				(string[])prms[3],
				(string[])prms[4]
				);
		}

		protected bool CheckOtherProccesses(string appName)
		{
			//Updating in progress do not interfere
			bool notAlredyRunning;
			_UpdatingFlag = new Mutex(true, "Global\\UpdateLib_" + appName, out notAlredyRunning);
			if (!notAlredyRunning)
				return false;

			//Install in progress do not interfere
			bool notInstAlredyRunning;
			var instUpdatingFlag = new Mutex(true, InstallInfo.InstallInfoMutexPrefix + appName, out notInstAlredyRunning);
			instUpdatingFlag.Close();
			if (!notInstAlredyRunning)
				return false;

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

		protected bool AskUserForDownload(string appName, VersionData versionInfo, string sourecUri)
		{
			return UIAskDownload.AskForDownload(appName, versionInfo, sourecUri);
		}

		protected string GetTempDir(string appName, Version latestVersion)
		{
			string path = Path.GetTempPath();
			path = Path.Combine(path, "UpdateLib");
			path = Path.Combine(path, appName);
			path = Path.Combine(path, latestVersion.ToString());

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

		protected bool DownloadVersion(VersionManifest downloadManifest, string tempPath)
		{
			if (UIDownloadProgress != null)
			{
			   DispatcherHelper.Invoke(new SimpleMathod(UIDownloadProgress.Show));
			   DispatcherHelper.Invoke(new SetDownloadProgressInfo(UIDownloadProgress.SetDownloadInfo),
					downloadManifest);
			}

			FileDownloader.DownloadFileStarted += (s, e) =>
				DispatcherHelper.Invoke(new UpdateDownloadProgress(
					UIDownloadProgress.SetDownloadProgress), e.FilePath, e.ToltalSize, e.DownloadedSize);

			var result = FileDownloader.DownloadFileSet(
				downloadManifest.VersionItems,
				tempPath);

			if (UIDownloadProgress != null)
				DispatcherHelper.Invoke(
					new SimpleMathod(UIDownloadProgress.Close));

			return result;
		}

		protected void VersionDownloadCompleted(
			string displayAppName, 
			VersionManifest downloadManifest,
			VersionManifest latestManifest,
			VersionData latestVersionInfo,
			InstallInfo installInfo)
		{
			if (!AskUserForInstall(displayAppName, latestVersionInfo))
				return;
			
			//Unzip
			foreach (var item in downloadManifest.VersionItems)
			{
				var tempFile = Path.Combine(installInfo.TempPath, item.GetItemFullPath());
				var tempFileUnzip = Path.Combine(installInfo.TempPath, item.GetUnzipItemFullPath());

				GZipCompression.DecompressFile(tempFile, tempFileUnzip);
			}

			//Saving latest manifest
			XmlSerializeHelper.SerializeItem(
				latestManifest,
				Path.Combine(installInfo.TempPath, VersionManifest.VersionManifestFileName));

			//Saving downloaded manifest
			XmlSerializeHelper.SerializeItem(
				downloadManifest,
				Path.Combine(installInfo.TempPath, VersionManifest.DownloadedVersionManifestFileName));

			var installerPath = CopyInstaller(installInfo.TempPath, installInfo.AppName, latestManifest.VersionNumber, installInfo);
			
			//Start installing
			Process.Start(installerPath);
			//_UpdatingFlag.Close();

			DispatcherHelper.Invoke(new SimpleMathod(OnNeedCloseApp));
		}

		protected string CopyInstaller(
			string tempPath, 
			string appName, 
			Version latestVersion, 
			InstallInfo installInfo)
		{
			var installerDir = GetInstallerDir(tempPath, appName, latestVersion);

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
				installInfo,
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

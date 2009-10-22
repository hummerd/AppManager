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
using UpdateLib.UI;
using UpdateLib.VersionInfo;


namespace UpdateLib
{
	public class SelfUpdate
	{
		public event EventHandler NeedCloseApp;
		public event EventHandler<UpdateCompleteInfo> UpdateCompleted;

		protected delegate void UpdateCompletedResult(bool successfulCheck, bool hasNewVersion, string message);
		protected delegate void UpdateDownloadProgress(string location, long total, long progress);
		protected delegate void SetDownloadProgressInfo(VersionManifest manifest);


		protected Mutex	_UpdatingFlag;
		protected Thread	_UpdateThread;

		protected volatile bool _UpdateRunning = false;


		public SelfUpdate()
		{
			FileDownloaderFactory = new FileDownloaderFactory();
			VNPFactory = new VersionNumberFactory();
			UIAskDownload = new UIAsk();
			UIAskInstall = new UIAsk();
			UIDownloadProgress = new UIDownloadProgress();
		}

		public SelfUpdate(
			IFileDownloaderFactory fileDownloadFactory, 
			IVersionNumberFactory vnpFactory,
			IUIAskDownload uiAskDownload,
			IUIAskInstall uiAskInstall,
			IUIDownloadProgress uiDownloadProgress
			)
		{
			FileDownloaderFactory = fileDownloadFactory;
			VNPFactory = vnpFactory;
			UIAskDownload = uiAskDownload;
			UIAskInstall = uiAskInstall;
			UIDownloadProgress = uiDownloadProgress;
		}
		

		//public IFileDownloader FileDownloader
		//{ get; set; }

		//public IVersionNumberProvider VersionNumberProvider
		//{ get; set; }

		public bool UpdateRunning
		{
			get
			{
				return _UpdateRunning;
			}
		}
		
		public IFileDownloaderFactory FileDownloaderFactory
		{ get; set; }

		public IVersionNumberFactory VNPFactory
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

		public Version GetCurrentVersion(string appPath)
		{
			VersionManifest currentManifest = GetCurrentVersionManifest(appPath);
			return currentManifest == null ? null : currentManifest.VersionNumber;
		}

		public bool UpdateAppAsync(
			string appName,
			string displayAppName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses,
			string defaultUpdateUri)
		{
			if (_UpdateThread != null && _UpdateThread.IsAlive)
				return false;

			_UpdateThread = new Thread(UpdateAppThread);
			_UpdateThread.SetApartmentState(ApartmentState.STA);
			_UpdateThread.IsBackground = true;
			_UpdateThread.Priority = ThreadPriority.BelowNormal;
			_UpdateThread.Start(new object[]
				{
				appName,
				displayAppName,
				appPath,
				executePaths,
				lockProcesses,
				defaultUpdateUri
				});
			return true;
		}

		public bool UpdateApp(
			string appName,
			string displayAppName,
			string appPath,
			string[] executePaths,
			string[] lockProcesses,
			string defaultUpdateUri)
		{
			try
			{
				VersionManifest currentManifest = GetCurrentVersionManifest(appPath);

				return UpdateApp(
					currentManifest,
					appName,
					displayAppName,
					appPath,
					executePaths,
					lockProcesses,
					defaultUpdateUri
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
			string[] lockProcesses,
			string defaultUpdateUri)
		{
			try
			{
				_UpdateRunning = true;

				//if update alredy running quit
				if (!CheckOtherProccesses(appName))
					return false;

				//if there is now cuurent version info supose we have first version
				if (currentManifest == null && !String.IsNullOrEmpty(defaultUpdateUri))
					currentManifest = new VersionManifest() { VersionNumberString = "1.0.0.0", UpdateUri = defaultUpdateUri };

				//clean up older install
				CleanUp(appName, currentManifest);

				Uri activeSource;
				IVersionNumberProvider vnp;
				var lastVersion = GetVersionData(new List<Uri>() 
					{	
						currentManifest.GetUpdateUriLocal(), 
						currentManifest.GetUpdateUriAltLocal() 
					},
					out activeSource,
					out vnp);

				if (lastVersion != null && lastVersion.VersionNumber > currentManifest.VersionNumber)
				{
					if (AskUserForDownload(displayAppName, lastVersion, activeSource.Authority))
					{
						var manifestUri = currentManifest.GetManifestUri(activeSource);

						VersionManifest latestManifest = vnp.GetLatestVersionManifest(manifestUri);
						//failed to download latest version manifest
						if (latestManifest == null)
							return false;

						VersionManifest updateManifest = latestManifest.GetUpdateManifest(currentManifest);

						var tempPath = CreateTempDir(appName, latestManifest.VersionNumber);
						DownloadVersion(updateManifest, tempPath, activeSource.AbsoluteUri);
						VersionDownloadCompleted(
							displayAppName,
							updateManifest,
							latestManifest,
							lastVersion,
							new InstallInfo()
							{
								AppName = appName,
								ExecutePaths = executePaths,
								InstallPath = appPath,
								LockProcess = lockProcesses,
								TempPath = tempPath
							});
					}
					else // there is new version but user dont wont to download it
					{
						InvokeUpdateCompleted(true, true, null);
					}
				}
				else if (lastVersion == null) //failed to get new version
					InvokeUpdateCompleted(false, false, null);
				//DispatcherHelper.Invoke(new UpdateCompletedResult(OnUpdateCompleted), false, false, null);
				else //there is no new version
					InvokeUpdateCompleted(true, false, null);
				//DispatcherHelper.Invoke(new UpdateCompletedResult(OnUpdateCompleted), true, false, null);
			}
			catch (UpdateException exc)
			{
				InvokeUpdateCompleted(false, false, exc.Message);
				return false;				
			}
			catch (Exception exc)
			{
				InvokeUpdateCompleted(false, false, exc.ToString());
				return false;
			}
			finally
			{
				if (_UpdatingFlag != null)
					_UpdatingFlag.Close();

				_UpdateRunning = false;
			}

			return true;
		}


		protected VersionData GetVersionData(IEnumerable<Uri> sources, out Uri activeSource, out IVersionNumberProvider vnp)
		{
			VersionData result = null;
			string msg = null;
			bool error = true;
			activeSource = null;
			vnp = null;

			foreach (var item in sources)
			{
				try
				{
					activeSource = item;
					vnp = VNPFactory.GetVNP(item.Scheme);
					result = vnp.GetLatestVersionInfo(item);
					if (result != null)
					{
						error = false;
						break;
					}
				}
				catch(Exception exc)
				{
					result = null;
					error = error && true;

					msg = msg + 
						item.AbsoluteUri + Environment.NewLine +
						exc.Message + Environment.NewLine + Environment.NewLine;
				}
			}

			if (error)
				throw new UpdateException(msg);

			return result;
		}

		protected VersionManifest GetCurrentVersionManifest(string appPath)
		{
			try
			{
				var manifestPath = Path.Combine(appPath, VersionManifest.VersionManifestFileName);

				if (File.Exists(manifestPath))
					return VersionManifestLoader.Load(manifestPath);
					//return XmlSerializeHelper.DeserializeItem(
					//   typeof(VersionManifest),
					//   manifestPath
					//   ) as VersionManifest;
			}
			catch 
			{ ; }

			return null;
		}

		protected void UpdateAppThread(object prm)
		{
			var prms = prm as object[];

			UpdateApp(
				(string)prms[0],
				(string)prms[1],
				(string)prms[2],
				(string[])prms[3],
				(string[])prms[4],
				(string)prms[5]
				);
		}

		protected bool CheckOtherProccesses(string appName)
		{
			//Updating in progress do not interfere
			bool notAlredyRunning;
			try
			{
				_UpdatingFlag = new Mutex(true, "Global\\UpdateLib_" + appName, out notAlredyRunning);
			}
			catch (UnauthorizedAccessException)
			{
				notAlredyRunning = false;
			}

			if (!notAlredyRunning)
				return false;

			//Install in progress do not interfere
			bool notInstAlredyRunning;
			Mutex instUpdatingFlag = null;
			try
			{
				instUpdatingFlag = new Mutex(true, InstallInfo.InstallInfoMutexPrefix + appName, out notInstAlredyRunning);
			}
			catch (UnauthorizedAccessException)
			{
				notInstAlredyRunning = false;
			}
			
			if (instUpdatingFlag != null)
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
				latestVersion + "_Inst");

			return installerDir;
		}

		protected bool AskUserForInstall(string appName, VersionData versionInfo)
		{
			return UIAskInstall.AskForInstall(appName, versionInfo);
		}

		protected void DownloadVersion(VersionManifest downloadManifest, string tempPath, string updateUri)
		{
			if (UIDownloadProgress != null)
			{
			   DispatcherHelper.Invoke(new SimpleMathod(UIDownloadProgress.Show));
			   DispatcherHelper.Invoke(new SetDownloadProgressInfo(UIDownloadProgress.SetDownloadInfo),
					downloadManifest);
			}

			try
			{
				var fileDownloader = FileDownloaderFactory.GetFileDownloader(new Uri(updateUri).Scheme);
				fileDownloader.DownloadFileStarted += (s, e) =>
					DispatcherHelper.Invoke(new UpdateDownloadProgress(
						UIDownloadProgress.SetDownloadProgress), e.FilePath, e.ToltalSize, e.DownloadedSize);

				fileDownloader.DownloadFileSet(
					downloadManifest.VersionItems,
					tempPath);
			}
			finally
			{
				if (UIDownloadProgress != null)
					DispatcherHelper.Invoke(
						new SimpleMathod(UIDownloadProgress.Close));
			}
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

			InvokeUpdateCompleted(true, true, null);
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

		protected void InvokeUpdateCompleted(bool successfulCheck, bool hasNewVersion, string msg)
		{
			DispatcherHelper.Invoke(new UpdateCompletedResult(OnUpdateCompleted), 
				successfulCheck, hasNewVersion, msg);
		}

		protected virtual void OnUpdateCompleted(bool successfulCheck, bool hasNewVersion, string msg)
		{
			if (UpdateCompleted != null)
				UpdateCompleted(this, new UpdateCompleteInfo()
					{ 
						SuccessfulCheck = successfulCheck,
						HasNewVersion = hasNewVersion,
						Message = msg
					});
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


	public class UpdateCompleteInfo : EventArgs
	{
		public bool SuccessfulCheck { get; set; }
		public bool HasNewVersion { get; set; }
		public string Message { get; set; }
	}
}

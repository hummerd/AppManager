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
		public static Version GetCurrentVersion(string appPath)
		{
			VersionManifest currentManifest = GetCurrentVersionManifest(appPath);
			return currentManifest == null ? null : currentManifest.VersionNumber;
		}

		private static VersionManifest GetCurrentVersionManifest(string appPath)
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


		public event EventHandler NeedCloseApp;
		public event EventHandler<UpdateCompleteInfo> UpdateCompleted;

		protected delegate void UpdateCompletedResult(bool successfulCheck, bool hasNewVersion, string message);
		protected delegate void UpdateDownloadProgress(string location, long total, long progress);
		protected delegate void SetDownloadProgressInfo(VersionManifest manifest);


		protected Mutex		_UpdatingFlag;
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
			VersionManifest latestManifest = null;
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

				var lastVersion = GetVersionData(new List<Uri>() 
					{	
						currentManifest.GetUpdateUriLocal(), 
						currentManifest.GetUpdateUriAltLocal() 
					});

				if (lastVersion != null && lastVersion.VersionData.VersionNumber > currentManifest.VersionNumber)
				{
					if (AskUserForDownload(displayAppName, lastVersion.VersionData, lastVersion.SourceUri.Authority))
					{
						var manifestUri = currentManifest.GetManifestUri(lastVersion.SourceUri);

						latestManifest = lastVersion.VNP.GetLatestVersionManifest(manifestUri);
						//failed to download latest version manifest
						if (latestManifest == null)
							return false;

						VersionManifest updateManifest = latestManifest.GetUpdateManifest(currentManifest);
					
						var tempPath = DownloadVersion(updateManifest, appName, lastVersion.VersionData.VersionNumber, lastVersion.SourceUri.AbsoluteUri);

						VersionDownloadCompleted(
							displayAppName,
							updateManifest,
							latestManifest,
							lastVersion.VersionData,
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
				CleanUp(appName, latestManifest);
				InvokeUpdateCompleted(false, false, exc.Message);
				return false;				
			}
			catch (Exception exc)
			{
				CleanUp(appName, latestManifest);
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

		/// <summary>
		/// Downloads VersionData from specified Uri's and returns version data with maximum version number
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="activeSource"></param>
		/// <param name="vnp"></param>
		/// <remarks>If download for all sources fails throws UpdateException</remarks>
		/// <returns>Returns version data with maximum version number</returns>
		protected VersionSource GetVersionData(IEnumerable<Uri> sources)
		{
			var result = new List<VersionSource>();
			string msg = null;
			bool error = true;

			foreach (var item in sources)
			{
				if (item == null)
					continue;

				try
				{
					var vnp = VNPFactory.GetVNP(item.Scheme);
					var vi = vnp.GetLatestVersionInfo(item);
					if (vi != null)
					{
						result.Add(
							new VersionSource(
								vi,
								item,
								vnp
								)
							);
						error = false;
						//break;
					}
				}
				catch(Exception exc)
				{
					error = error && true;

					msg = msg + 
						item.AbsoluteUri + Environment.NewLine +
						exc.Message + Environment.NewLine + Environment.NewLine;
				}
			}

			if (error)
				throw new UpdateException(msg);

			if (result.Count <= 0)
				return null;
			else if (result.Count == 1)
				return result[0];
			else
			{
				var vi = result[0];
				for (int i = 1; i < result.Count; i++)
					if (result[i].VersionData.VersionNumber > vi.VersionData.VersionNumber)
						vi = result[i];

				return vi;
			}
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

		protected void CleanUp(string appName, VersionManifest manifest)
		{
			if (manifest == null)
				return;

			try
			{
				var tempPath = GetTempVersionDir(appName, manifest.VersionNumber);
				var instDir = GetInstallerDir(tempPath, appName, manifest.VersionNumber);

				if (Directory.Exists(tempPath))
					Directory.Delete(tempPath, true);

				if (Directory.Exists(instDir))
					Directory.Delete(instDir, true);

				//Delete old trash
				//probably it was unseccesfull installations
				var tmpDir = GetTempDir(appName);
				if (Directory.Exists(tmpDir))
				{
					var tmp = new DirectoryInfo(tmpDir);
					var dirs = tmp.GetDirectories();
					Array.ForEach(dirs, d => d.Delete(true));
				}
			}
			catch
			{ ; }
		}

		protected bool AskUserForDownload(string appName, VersionData versionInfo, string sourecUri)
		{
			return UIAskDownload.AskForDownload(appName, versionInfo, sourecUri);
		}

		protected string GetTempDir(string appName)
		{
			string path = Path.GetTempPath();
			path = Path.Combine(path, "UpdateLib");
			path = Path.Combine(path, appName);

			return path;
		}

		protected string GetTempVersionDir(string appName, Version latestVersion)
		{
			var path = Path.Combine(GetTempDir(appName), latestVersion.ToString());
			return path;
		}

		protected string CreateTempDir(string appName, Version latestVersion)
		{
			string path = GetTempVersionDir(appName, latestVersion);

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

		protected string DownloadVersion(
			VersionManifest downloadManifest, 
			string appName,
			Version latestVersion,
			string updateUri
			)
		{
			var tempApplicationPath = String.Empty;

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

				var downloadItems = new List<LocationHash>(downloadManifest.VersionItems.Count);
				foreach (var item in downloadManifest.VersionItems)
				{
					if (item.InstallAction != InstallAction.Delete)
					downloadItems.Add(item.GetLocationHash());
				}

				tempApplicationPath = CreateTempDir(appName, latestVersion);
				fileDownloader.DownloadFileSet(
					downloadItems,
					tempApplicationPath);

				var tempInstallerPath = CreateInstallerDir(tempApplicationPath, appName, latestVersion);
				fileDownloader.DownloadFileSet(
					downloadManifest.BootStrapper,
					tempInstallerPath);
			}
			finally
			{
				if (UIDownloadProgress != null)
					DispatcherHelper.Invoke(
						new SimpleMathod(UIDownloadProgress.Close));
			}

			return tempApplicationPath;
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
			
			//Unzip downloaded version
			foreach (var item in downloadManifest.VersionItems)
			{
				var tempFile = Path.Combine(installInfo.TempPath, item.GetItemFullPath());
				var tempFileUnzip = Path.Combine(installInfo.TempPath, item.GetUnzipItemFullPath());

				GZipCompression.DecompressFile(tempFile, tempFileUnzip);
			}

			LocationHash badLocation;
			if (!CheckHash(installInfo.TempPath, downloadManifest.VersionItems.ConvertAll(vi => vi.GetLocationHash()), out badLocation))
			{
				throw new UpdateException(UpdStr.BAD_VERSION);
			}
			
			//Unzip bootstrapper
			var tempInstallerPath = GetInstallerDir(installInfo.TempPath, installInfo.AppName, latestManifest.VersionNumber);
			foreach (var item in downloadManifest.BootStrapper)
			{
				var tempFile = Path.Combine(tempInstallerPath, item.GetItemFullPath());
				var tempFileUnzip = Path.Combine(tempInstallerPath, item.GetUnzipItemFullPath());

				GZipCompression.DecompressFile(tempFile, tempFileUnzip);
			}

			if (!CheckHash(tempInstallerPath, downloadManifest.BootStrapper, out badLocation))
			{
				throw new UpdateException(UpdStr.BAD_VERSION);
			}
			
			//Saving latest manifest
			XmlSerializeHelper.SerializeItem(
				latestManifest,
				Path.Combine(installInfo.TempPath, VersionManifest.VersionManifestFileName));

			//Saving downloaded manifest
			XmlSerializeHelper.SerializeItem(
				downloadManifest,
				Path.Combine(installInfo.TempPath, VersionManifest.DownloadedVersionManifestFileName));

			//var installerPath = CopyInstaller(installInfo.TempPath, installInfo.AppName, latestManifest.VersionNumber, installInfo);
			var installerDir = GetInstallerDir(installInfo.TempPath, installInfo.AppName, latestManifest.VersionNumber);
			var installerPath = Path.Combine(installerDir, "Updater.exe");

			//Saving install info
			XmlSerializeHelper.SerializeItem(
				installInfo,
				Path.Combine(installerDir, InstallInfo.InstallInfoFileName));

			//Start installing
			Process.Start(installerPath);
			//_UpdatingFlag.Close();

			InvokeUpdateCompleted(true, true, null);
			DispatcherHelper.Invoke(new SimpleMathod(OnNeedCloseApp));
		}

		protected bool CheckHash(string tempPath, List<LocationHash> versionItems, out LocationHash badOne)
		{
			badOne = null;

			foreach (var item in versionItems)
			{
				//if (item.InstallAction == InstallAction.Delete)
				//    continue;

				if (FileHash.GetBase64FileHash(Path.Combine(tempPath, item.GetUnzipItemFullPath())) !=
					item.Base64Hash)
				{
					badOne = item;
					return false;
				}
			}

			return true;
		}

		protected string CreateInstallerDir(
			string tempPath, 
			string appName, 
			Version latestVersion)
		{
			var installerDir = GetInstallerDir(tempPath, appName, latestVersion);

			if (Directory.Exists(installerDir))
				Directory.Delete(installerDir, true);

			Directory.CreateDirectory(installerDir);
			return installerDir;
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

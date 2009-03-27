using System;


namespace UpdateLib.VersionInfo
{
	public class VersionDownloadInfo : EventArgs
	{
		public bool Succeded
		{ get; set; }
		public VersionManifest DownloadedVersionManifest
		{ get; set; }
		public VersionManifest LatestVersionManifest
		{ get; set; }
		public VersionData LatestVersionInfo
		{ get; set; }
		public VersionManifest CurrentVersionManifest
		{ get; set; }
		public string AppName
		{ get; set; }
		public string DisplayAppName
		{ get; set; }
		public string AppPath
		{ get; set; }
		public string[] ExecutePaths
		{ get; set; }
		public string[] LockProcesses
		{ get; set; }
		public string TempPath
		{ get; set; }
	}
}

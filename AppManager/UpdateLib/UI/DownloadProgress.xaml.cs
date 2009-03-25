using System.Windows;
using UpdateLib.VersionInfo;
using System;


namespace UpdateLib.UI
{
	/// <summary>
	/// Interaction logic for DownloadProgress.xaml
	/// </summary>
	public partial class DownloadProgress : Window, IUIDownloadProgress
	{
		public DownloadProgress()
		{
			InitializeComponent();
			LblDownload.Content = UpdStr.DOWNLOADING;
			Title = UpdStr.UPDATER;
		}

		#region IUIDownloadProgress Members

		public void SetDownloadInfo(VersionManifest manifest)
		{
			PrgTotal.Maximum = manifest.VersionItems.Count;
		}

		public void SetDownloadProgress(string location, long total, long progress)
		{
			if (progress == 0)
				PrgTotal.Value += 1;

			PrgFile.Value = progress;
			var loc = new Uri(location);
			LblFile.Content = loc.Segments[loc.Segments.Length - 1];
		}

		#endregion
	}
}

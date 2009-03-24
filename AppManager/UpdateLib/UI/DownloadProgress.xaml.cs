using System.Windows;
using UpdateLib.VersionNumberProvider;


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

			//PrgFile.Maximum = total;
			PrgFile.Value = progress;
			LblFile.Content = location;
		}

		#endregion
	}
}

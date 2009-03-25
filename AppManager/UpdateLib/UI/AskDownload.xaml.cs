using System.Windows;
using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	/// <summary>
	/// Interaction logic for AskDownload.xaml
	/// </summary>
	public partial class AskDownload : Window, IUIAskDownload, IUIAskInstall
	{
		public AskDownload()
		{
			InitializeComponent();
		}

		#region IUIAskDownload Members

		public bool AskForDownload(VersionData versionInfo)
		{
			return ShowDialog() ?? false;
		}

		#endregion

		#region IUIAskInstall Members

		public bool AskForInstall(VersionData versionInfo)
		{
			return ShowDialog() ?? false;
		}

		#endregion

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}

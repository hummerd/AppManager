using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Windows;
using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public class UIAsk : MessageBox, IUIAskDownload, IUIAskInstall
	{
		#region IUIAskDownload Members

		public bool AskForDownload(string appName, VersionData versionInfo)
		{
			Title = UpdStr.UPDATER;
			Message = String.Format(
				UpdStr.NEW_VER_AVIALABLE, 
				versionInfo.VersionNumber,
				appName,
				versionInfo.Description);
			WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			return ShowDialog() ?? false;
		}

		#endregion

		#region IUIAskInstall Members

		public bool AskForInstall(string appName, VersionData versionInfo)
		{
			Title = UpdStr.UPDATER;
			Message = String.Format(
				UpdStr.ASK_FOR_INSTALL,
				appName,
				versionInfo.VersionNumber);
			WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			return ShowDialog() ?? false;
		}

		#endregion
	}
}

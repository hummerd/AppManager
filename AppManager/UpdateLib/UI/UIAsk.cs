using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Windows;
using UpdateLib.VersionInfo;


namespace UpdateLib.UI
{
	public class UIAsk : IUIAskDownload, IUIAskInstall
	{
		#region IUIAskDownload Members

		public bool AskForDownload(string appName, VersionData versionInfo)
		{
			var msgb = new MessageBox();

			msgb.Topmost = true;
			msgb.Title = UpdStr.UPDATER;
			msgb.Message = String.Format(
				UpdStr.NEW_VER_AVIALABLE, 
				versionInfo.VersionNumber,
				appName,
				versionInfo.Description);
			msgb.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			return msgb.ShowDialog() ?? false;
		}

		#endregion

		#region IUIAskInstall Members

		public bool AskForInstall(string appName, VersionData versionInfo)
		{
			var msgb = new MessageBox();

			msgb.Topmost = true;
			msgb.Title = UpdStr.UPDATER;
			msgb.Message = String.Format(
				UpdStr.ASK_FOR_INSTALL,
				appName,
				versionInfo.VersionNumber);
			msgb.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			return msgb.ShowDialog() ?? false;
		}

		#endregion
	}
}

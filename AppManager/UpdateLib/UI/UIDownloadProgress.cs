using System;
using System.Collections.Generic;
using System.Text;
using UpdateLib.VersionInfo;

namespace UpdateLib.UI
{
	public class UIDownloadProgress : IUIDownloadProgress
	{
		protected DownloadProgress _Window;


		#region IUIDownloadProgress Members

		public void Show()
		{
			if (_Window != null)
				_Window.Close();

			_Window = new DownloadProgress();
			_Window.Show();
		}

		public void SetDownloadInfo(VersionManifest manifest)
		{
			_Window.SetDownloadInfo(manifest);
		}

		public void SetDownloadProgress(string location, long total, long progress)
		{
			_Window.SetDownloadProgress(location, total, progress);
		}

		public void Close()
		{
			_Window.Close();
			_Window = null;
		}

		#endregion
	}
}

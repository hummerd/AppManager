using System.Collections.Generic;
using System.IO;
using UpdateLib.Install;
using System.Diagnostics;


namespace Updater
{
	public class InstallHelper
	{
		protected InstallManifest		_Manifest;
		protected List<InstallItem>	_RunItems = new List<InstallItem>();


		public InstallHelper(InstallManifest manifest)
		{
			_Manifest = manifest;
		}


		public void Install()
		{
			_RunItems.Clear();

			foreach (var item in _Manifest.InstallItems)
			{
				File.Copy(item.SrcPath, item.DstPath, true);

				if (item.InstallAction == InstallAction.CopyAndRun)
					_RunItems.Add(item);
			}

			foreach (var item in _RunItems)
				StartItem(item);

			foreach (var item in _Manifest.InstallItems)
				File.Delete(item.SrcPath);
		}


		protected void StartItem(InstallItem item)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = item.DstPath;
			psi.WorkingDirectory = Path.GetDirectoryName(item.DstPath);
			Process.Start(psi);
		}
	}
}

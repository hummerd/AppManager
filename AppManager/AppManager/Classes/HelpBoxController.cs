using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using AppManager.Properties;
using CommonLib.Windows;
using UpdateLib;


namespace AppManager
{
	public class HelpBoxController : ControllerBase, IDisposable
	{
		public event EventHandler UpdateCheckCompleted;


		public HelpBoxController(MainWorkItem workItem)
			: base(workItem)
		{
			_WorkItem.UpdateCompleted += WorkItem_UpdateCompleted;
		}


		public string GetVersionString()
		{
			var ver = SelfUpdate.GetCurrentVersion(_WorkItem.AppPath);
			if (ver == null)
				ver = Assembly.GetEntryAssembly().GetName().Version;

			return Strings.APP_TITLE + " " + ver;
		}

		public void GoToAppPage()
		{
			try
			{
				Process.Start(Resources.APP_PAGE);
			}
			catch (Exception exc)
			{
				ErrorBox.Show(Strings.APP_MANAGER, exc);
			}
		}

		public FlowDocument GetHelpText()
		{
			var res = Application.GetResourceStream(new Uri(Strings.HELP_FILE, UriKind.Relative));

			var fd = new FlowDocument();
			var tb = new TextRange(fd.ContentStart, fd.ContentEnd);
			tb.Load(res.Stream, DataFormats.Rtf);

			return fd;
		}

		public void CheckNewVersion()
		{
			_WorkItem.Commands.CheckVersion.Execute(null);
		}


		protected virtual void OnUpdateCheckCompleted()
		{
			if (UpdateCheckCompleted != null)
				UpdateCheckCompleted(this, EventArgs.Empty);
		}


		private void WorkItem_UpdateCompleted(object sender, EventArgs e)
		{
			OnUpdateCheckCompleted();
		}


		#region IDisposable Members

		public void Dispose()
		{
			_WorkItem.UpdateCompleted -= WorkItem_UpdateCompleted;
		}

		#endregion
	}
}

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using AppManager.Properties;


namespace AppManager
{
	public class HelpBoxController : ControllerBase
	{
		public event EventHandler UpdateCheckCompleted;


		public HelpBoxController(MainWorkItem workItem)
			: base(workItem)
		{
			_WorkItem.Updater.UpdateCompleted += (s, e) => OnUpdateCheckCompleted();
		}


		public string GetVersionString()
		{
			var ver = _WorkItem.Updater.GetCurrentVersion(_WorkItem.AppPath);
			if (ver == null)
				ver = Assembly.GetEntryAssembly().GetName().Version;

			return Strings.APP_TITLE + " " + ver;
		}

		public void GoToAppPage()
		{
			Process.Start(Resources.APP_PAGE);
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
			_WorkItem.Updater.UpdateAppAsync(
				"AppManager",
				Strings.APP_TITLE,
				_WorkItem.AppPath,
				new string[] { Assembly.GetExecutingAssembly().Location },
				new string[] { Process.GetCurrentProcess().ProcessName },
				"http://hummerd.com/AppManagerUpdate"
				);
		}


		protected virtual void OnUpdateCheckCompleted()
		{
			if (UpdateCheckCompleted != null)
				UpdateCheckCompleted(this, EventArgs.Empty);
		}
	}
}

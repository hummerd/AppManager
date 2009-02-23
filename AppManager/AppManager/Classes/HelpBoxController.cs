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
		public HelpBoxController(MainWorkItem workItem)
			: base(workItem)
		{ }


		public string GetVersionString()
		{
			return Strings.APP_TITLE + " " +
				Assembly.GetEntryAssembly().GetName().Version;
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
	}
}

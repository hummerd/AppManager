using System;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using CommonLib.Windows;


namespace AppManager.Windows
{
    /// <summary>
    /// Interaction logic for HelpBox.xaml
    /// </summary>
	public partial class HelpBox : Window
	{
		public HelpBox()
		{
			InitializeComponent();

			RunVersion.Text = Strings.APP_TITLE +
				Assembly.GetEntryAssembly().GetName().Version;

			LoadHelpText();

			new DialogKeyDecorator(this, BtnOk, null, false);
		}


		protected void LoadHelpText()
		{
			var res = Application.GetResourceStream(new Uri(Strings.HELP_FILE, UriKind.Relative));

			var fd = new FlowDocument();
			var tb = new TextRange(fd.ContentStart, fd.ContentEnd);
			tb.Load(res.Stream, DataFormats.Rtf);

			HelpText.Document = fd;
		}
	}
}

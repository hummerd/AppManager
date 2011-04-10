using System.Windows;
using System;
using System.Diagnostics;
using System.Reflection;


namespace CommonLib.Windows
{
	/// <summary>
	/// Interaction logic for ErrorBox.xaml
	/// </summary>
	public partial class ErrorBox : Window
	{
		public static void Show(string title, string message, string details)
		{
			var eb = new ErrorBox(title, message, details);
			eb.ShowDialog();
		}

		public static void Show(string title, Exception exc)
		{
			try
			{
				var srcName = Assembly.GetEntryAssembly().GetName().Name;
				try
				{
					if (!EventLog.Exists(srcName))
						EventLog.WriteEntry(srcName, exc.ToString(), EventLogEntryType.Error);
				}
				catch
				{ ; }

				var eb = new ErrorBox(title, exc.Message, exc.ToString());
				eb.ShowDialog();
			}
			catch
			{ ; }
		}


		public ErrorBox(string title, string message, string details)
		{
			InitializeComponent();

			Title = title;
			TxtMessage.Text = message;
			TxtDetails.Text = details;

			new DialogKeyDecorator(this, BtnOk, null, false);
		}

		private void ExpDetails_Expanded(object sender, RoutedEventArgs e)
		{
			//ExpDetails.Height = 136;
			//MainGrid.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
			ExpDetails.VerticalAlignment = VerticalAlignment.Stretch;
			if (Height <= 200)
				Height = 300;
		}

		private void ExpDetails_Collapsed(object sender, RoutedEventArgs e)
		{
			//ExpDetails.Height = 36;
			//MainGrid.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Auto);
			ExpDetails.VerticalAlignment = VerticalAlignment.Top;
			SizeToContent = SizeToContent.Height;
		}
	}
}

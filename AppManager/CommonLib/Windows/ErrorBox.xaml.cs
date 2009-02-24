using System.Windows;


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


		public ErrorBox(string title, string message, string details)
		{
			InitializeComponent();

			Title = title;
			TxtMessage.Text = message;
			TxtDetails.Text = details;

			new DialogKeyDecorator(this, BtnOk, null, false);
		}
	}
}

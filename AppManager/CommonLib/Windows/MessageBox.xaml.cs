using System.Windows;


namespace CommonLib.Windows
{
	/// <summary>
	/// Interaction logic for MessageBox.xaml
	/// </summary>
	public partial class MsgBox : Window
	{
		public static bool Show(Window owner, string title, string message)
		{
			return Show(owner, title, message, true);
		}

		public static bool Show(Window owner, string title, string message, bool yesNo)
		{
			var msg = new MsgBox();
			msg.Title = title;
			msg.Message = message;

			msg.BtnYes.Visibility = yesNo ? Visibility.Visible : Visibility.Collapsed;
			msg.BtnNo.Visibility = yesNo ? Visibility.Visible : Visibility.Collapsed;
			msg.BtnOk.Visibility = yesNo ? Visibility.Collapsed : Visibility.Visible;

			msg.Owner = owner;
			msg.WindowStartupLocation = owner == null ?
				WindowStartupLocation.CenterScreen :
				WindowStartupLocation.CenterOwner;

			return msg.ShowDialog() ?? false;
		}


		public MsgBox()
		{
			InitializeComponent();

			new DialogKeyDecorator(this, BtnYes, BtnNo, true);
		}


		public string Message
		{
			get { return LblHeader.Text as string; }
			set { LblHeader.Text = value; } 
		}


		private void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}

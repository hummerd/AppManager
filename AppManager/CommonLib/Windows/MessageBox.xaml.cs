using System.Windows;


namespace CommonLib.Windows
{
	/// <summary>
	/// Interaction logic for MessageBox.xaml
	/// </summary>
	public partial class MessageBox : Window
	{
		public static bool Show(Window owner, string title, string message)
		{
			var msg = new MessageBox();
			msg.Title = title;
			msg.Message = message;

			msg.Owner = owner;
			msg.WindowStartupLocation = owner == null ?
				WindowStartupLocation.CenterScreen :
				WindowStartupLocation.CenterOwner;

			return msg.ShowDialog() ?? false;
		}


		public MessageBox()
		{
			InitializeComponent();

			new DialogKeyDecorator(this, BtnYes, BtnNo, true);
		}


		public string Message 
		{
			get { return LblHeader.Text as string; }
			set { LblHeader.Text = value; } 
		}
	}
}

using System;
using System.Windows;
using System.Windows.Input;
using CommonLib.Windows;


namespace CommonLib.Windows
{
	/// <summary>
	/// Interaction logic for InputBox.xaml
	/// </summary>
	public partial class InputBox : DialogWindow
	{
		public InputBox(string caption)
		{
			InitializeComponent();
			Title = caption;
		}


		public string InputText
		{ 
			get
			{
				return TxtInput.Text;
			}
			set
			{
				TxtInput.Text = value;
			}
		}


		private void Window_Activated(object sender, EventArgs e)
		{
			TxtInput.Focus();
			TxtInput.SelectAll();
		}
	}
}

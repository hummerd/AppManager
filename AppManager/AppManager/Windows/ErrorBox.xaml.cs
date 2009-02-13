﻿using System.Windows;


namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for ErrorBox.xaml
	/// </summary>
	public partial class ErrorBox : Window
	{
		public static void Show(string title, string message, string details)
		{
			var eb = new ErrorBox(title, message, details);
			eb.Show();
		}


		public ErrorBox(string title, string message, string details)
		{
			InitializeComponent();

			Title = title;
			TxtMessage.Text = message;
			TxtDetails.Text = details;
		}


		private void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}

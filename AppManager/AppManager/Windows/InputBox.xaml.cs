﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppManager.Windows
{
	/// <summary>
	/// Interaction logic for InputBox.xaml
	/// </summary>
	public partial class InputBox : Window
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


		private void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			TxtInput.Focus();
			TxtInput.SelectAll();
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				DialogResult = true;
			}

			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				DialogResult = false;
			}
		}
	}
}
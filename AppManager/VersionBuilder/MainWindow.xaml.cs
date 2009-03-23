﻿using System;
using System.Windows;
using CommonLib.Windows;


namespace VersionBuilder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		protected VersionFactory _VesionFactory = new VersionFactory();


		public MainWindow()
		{
			InitializeComponent();
		}


		private void btnCreateVersion_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Version ver = new Version(TxtVersion.Text);
				_VesionFactory.CreateVersion(TxtSourceDir.Text, ver, @"http://localhost/AppManagerVersion");
			}
			catch(Exception exc)
			{
				ErrorBox.Show("Error", exc.Message, exc.ToString());
			}
		}
	}
}

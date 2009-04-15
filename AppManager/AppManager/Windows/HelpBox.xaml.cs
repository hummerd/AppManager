﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using CommonLib.Windows;
using System.Windows.Navigation;


namespace AppManager.Windows
{
	/// <summary>
   /// Interaction logic for HelpBox.xaml
   /// </summary>
	public partial class HelpBox : Window
	{
		protected HelpBoxController _Controller;


		public HelpBox(MainWorkItem workItem, bool about)
		{
			_Controller = new HelpBoxController(workItem);
			_Controller.UpdateCheckCompleted += (s, e) => SetUpdateState(true);

			InitializeComponent();

			AppTabs.SelectedIndex = about ? 0 : 1;
			RunVersion.Content = _Controller.GetVersionString();
			HelpText.Document = _Controller.GetHelpText();
			SetUpdateState(!_Controller.WorkItem.Updater.UpdateRunning);

			new DialogKeyDecorator(this, BtnOk, null, false);
		}


		protected void SetUpdateState(bool enabled)
		{
			BtnCheckNewVersion.IsEnabled = enabled;
			BtnCheckNewVersion.Content = enabled ?
				Strings.CHECK_FOR_UPDATE :
				Strings.LOOK_FOR_NEW_VER;
		}


		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			_Controller.GoToAppPage();
		}

		private void BtnCheckNewVersion_Click(object sender, RoutedEventArgs e)
		{
			SetUpdateState(false);
			_Controller.CheckNewVersion();
		}
	}
}

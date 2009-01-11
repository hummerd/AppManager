using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using AppManager.Settings;


namespace AppManager.Commands
{
	public class QuitApp : CommandBase
	{
		public QuitApp(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{
			SaveData();

			_WorkItem.MainWindow.SaveState();
			_WorkItem.TrayIcon.Dispose();
			_WorkItem.KbrdHook.Dispose();

			AMSetttingsFactory.DefaultSettingsBag.SaveSettings("appsettings.xml");
			App.Current.Shutdown();
		}


		protected void SaveData()
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlSerializer xser = new XmlSerializer(_WorkItem.AppData.GetType());

			using (StringWriter sr = new StringWriter())
			{
				xser.Serialize(sr, _WorkItem.AppData);
				xmlDoc.LoadXml(sr.ToString());
			}

			xmlDoc.Save(_WorkItem.DataPath);
		}
	}
}

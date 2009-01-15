using System.IO;
using System.Xml;
using System.Xml.Serialization;
using AppManager.Settings;


namespace AppManager.Commands
{
	public class Save: CommandBase
	{
		public Save(MainWorkItem workItem)
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
			AMSetttingsFactory.DefaultSettingsBag.SaveSettings("appsettings.xml");
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

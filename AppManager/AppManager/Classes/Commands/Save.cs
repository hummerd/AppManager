using System.IO;
using System.Xml;
using System.Xml.Serialization;
using AppManager.Settings;
using CommonLib;


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
			XmlSerializeHelper.SerializeItem(
				_WorkItem.AppData,
				_WorkItem.DataPath);

			_WorkItem.MainWindow.SaveState();
			AMSetttingsFactory.DefaultSettingsBag.SaveSettings("appsettings.xml");
		}
	}
}

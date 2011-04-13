using AppManager.Settings;


namespace AppManager.Commands
{
	public class Save: CommandBase
	{
		public Save(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			//XmlSerializeHelper.SerializeItem(
			//   _WorkItem.AppData,
			//   _WorkItem.DataPath);

			AppGroupLoader.Save(_WorkItem.DataPath, _WorkItem.AppData);
			AppGroupLoader.SaveRecycleBin(_WorkItem.RecycleBinPath, _WorkItem.RecycleBin);
			AppGroupLoader.SaveAppStat(_WorkItem.StatPath, _WorkItem.AppData);

			_WorkItem.MainWindow.SaveState();
			AMSetttingsFactory.DefaultSettingsBag.SaveSettings("appsettings.xml");
		}
	}
}

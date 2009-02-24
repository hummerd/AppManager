using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace AppManager.Commands
{
	public class AppCommands
	{
		protected MainWorkItem _WorkItem;


		public AppCommands(MainWorkItem workItem)
		{
			_WorkItem = workItem;

			Start = new StartApp(_WorkItem);
			Quit = new QuitApp(_WorkItem);
			RunApp = new RunApp(_WorkItem);
			Activate = new Activate(_WorkItem);
			Deactivate = new Deactivate(_WorkItem);
			ManageApps = new ManageApps(_WorkItem);
			Settings = new Settings(_WorkItem);
			Save = new Save(_WorkItem);
			Help = new Help(_WorkItem);
		}


		public ICommand Start { get; set; }
		public ICommand Quit { get; set; }
		public ICommand Activate { get; set; }
		public ICommand Deactivate { get; set; }
		public ICommand RunApp { get; set; }
		public ICommand ManageApps { get; set; }
		public ICommand Settings { get; set; }
		public ICommand Save { get; set; }
		public ICommand Help { get; set; }
	}
}

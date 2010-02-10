using CommonLib.PInvoke;

namespace AppManager.Commands
{
	public class Deactivate : CommandBase
	{
		public Deactivate(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			_WorkItem.MainWindow.Hide();
			Kernel32.GropWorkingSet();
		}
	}
}

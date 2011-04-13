

namespace AppManager
{
	public class ControllerBase
	{
		protected MainWorkItem _WorkItem;


		public ControllerBase(MainWorkItem workItem)
		{
			_WorkItem = workItem;
		}


		public MainWorkItem WorkItem
		{ 
			get
			{
				return _WorkItem;
			}
		}
	}
}

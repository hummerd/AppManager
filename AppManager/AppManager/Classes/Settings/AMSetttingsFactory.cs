
namespace AppManager.Settings
{
	public class AMSetttingsFactory
	{
		public static MainWorkItem WorkItem;


		private static SettingsBag _DefaultSettingsBag = null;

		public static SettingsBag DefaultSettingsBag
		{
			get
			{
				if (_DefaultSettingsBag == null)
				{
					AMSettingLoader settLoader = new AMSettingLoader();
					settLoader.WorkItem = WorkItem;
					_DefaultSettingsBag = new SettingsBag(settLoader);
					_DefaultSettingsBag.LoadSettings("appsettings.xml");
				}

				return _DefaultSettingsBag;
			}
		}
	}
}

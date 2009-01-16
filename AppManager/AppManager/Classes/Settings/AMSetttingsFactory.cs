
namespace AppManager.Settings
{
	public class AMSetttingsFactory
	{
		public static MainWorkItem WorkItem;


		private static SettingsBag<AppManagerSettings> _DefaultSettingsBag = null;

		public static SettingsBag<AppManagerSettings> DefaultSettingsBag
		{
			get
			{
				if (_DefaultSettingsBag == null)
				{
					AMSettingLoader settLoader = new AMSettingLoader();
					settLoader.WorkItem = WorkItem;
					_DefaultSettingsBag = new SettingsBag<AppManagerSettings>(settLoader);
					_DefaultSettingsBag.LoadSettings("appsettings.xml");
				}

				return _DefaultSettingsBag;
			}
		}
	}
}

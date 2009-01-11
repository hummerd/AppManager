using System.Windows;


namespace AppManager.Settings
{
	public struct WndSettings
	{
		public static WndSettings Empty = new WndSettings();

		public Point Location;
		public Size Size;
	}


	public class WndSettingsAdapter : ControlAdapterBase<Window>
	{
		private static WndSettingsAdapter _Instance;

		public static WndSettingsAdapter Instance 
		{
			get
			{
				if (_Instance == null)
					_Instance = new WndSettingsAdapter();

				return _Instance; 
			}
		}


		public override void SaveControlSettings(Window control, string settingName, SettingsBag settings)
		{
			WndSettings setting = new WndSettings();
			setting.Location = new Point(control.Left, control.Top);
			setting.Size = new Size(control.Width, control.Height);

			settings.SetSetting(settingName, setting);
		}

		protected override void LoadControlSettings(Window control, string settingName, SettingsBag settings)
		{
			WndSettings setting = settings.GetSettingCmplx<WndSettings>(settingName, WndSettings.Empty);

			if (!setting.Equals(WndSettings.Empty))
			{
				control.WindowStartupLocation = WindowStartupLocation.Manual;
				control.Left = setting.Location.X;
				control.Top = setting.Location.Y;
				control.Width = setting.Size.Width;
				control.Height = setting.Size.Height;
			}
		}
	}
}

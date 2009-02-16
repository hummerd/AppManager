using System.Windows;
using CommonLib;


namespace AppManager.Settings
{
	public struct WndSettings
	{
		public static WndSettings Empty = new WndSettings();

		public Point Location;
		public Size Size;
	}


	public class WndSettingsAdapter<TSettings> : ControlAdapterBase<Window, TSettings>
	{
		private static WndSettingsAdapter<TSettings> _Instance;

		public static WndSettingsAdapter<TSettings> Instance 
		{
			get
			{
				if (_Instance == null)
					_Instance = new WndSettingsAdapter<TSettings>();

				return _Instance; 
			}
		}


		public override void SaveControlSettings(
			Window control, 
			string settingName,
			SettingsBag<TSettings> settings)
		{
			WndSettings setting = new WndSettings();
			setting.Location = new Point(control.Left, control.Top);
			setting.Size = new Size(control.Width, control.Height);

			PropSetter.SetValue(settings.Settings, settingName, setting);
		}

		protected override void LoadControlSettings(
			Window control, 
			string settingName,
			SettingsBag<TSettings> settings)
		{
			WndSettings setting = PropSetter.GetValue<WndSettings>(settings.Settings, settingName);
			//WndSettings setting = settings.GetSettingCmplx<WndSettings>(settingName, WndSettings.Empty);

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

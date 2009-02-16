using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using CommonLib;


namespace AppManager.Settings
{
	public abstract class ControlAdapterBase<TControl, TSettings>
		where TControl : DependencyObject
	{
		private Dictionary<Window, WndSettingsParams> _Context = new Dictionary<Window, WndSettingsParams>();


		public void SetControlSettings(TControl control, string settingName, SettingsBag<TSettings> settings)
		{
			SetControlSettings(control, settingName, settings, true);
		}

		public void SetControlSettings(TControl control, string settingName, SettingsBag<TSettings> settings, bool saveOnClose)
		{
			LoadControlSettings(control, settingName, settings);

			if (saveOnClose)
			{
				Window wnd = UIHelper.FindAncestorOrSelf<Window>(control);
				wnd.Closing -= WndClosing;
				wnd.Closing += WndClosing;

				var fs = new WndSettingsParams()
				{
					Settings = settings,
					SettingName = settingName,
					Control = control
				};

				_Context[wnd] = fs;
			}
		}

		public abstract void SaveControlSettings(TControl control, string settingName, SettingsBag<TSettings> settings);


		protected abstract void LoadControlSettings(TControl control, string settingName, SettingsBag<TSettings> settings);


		protected void WndClosing(object sender, CancelEventArgs e)
		{
			Window wnd = sender as Window;
			wnd.Closing -= WndClosing;

			WndSettingsParams fs = _Context[wnd];
			_Context.Remove(wnd);
			SaveControlSettings(fs.Control, fs.SettingName, fs.Settings);
		}


		protected class WndSettingsParams
		{
			public SettingsBag<TSettings> Settings { get; set; }
			public string SettingName { get; set; }
			public TControl Control { get; set; }
		}
	}
}

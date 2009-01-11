using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AppManager.Common;


namespace AppManager.Settings
{
	public abstract class ControlAdapterBase<T>
		where T : DependencyObject
	{
		private Dictionary<Window, WndSettingsParams<T>> _Context = new Dictionary<Window, WndSettingsParams<T>>();


		public void SetControlSettings(T control, string settingName, SettingsBag settings)
		{
			SetControlSettings(control, settingName, settings, true);
		}

		public void SetControlSettings(T control, string settingName, SettingsBag settings, bool saveOnClose)
		{
			LoadControlSettings(control, settingName, settings);

			if (saveOnClose)
			{
				Window wnd = UIHelper.FindAncestorOrSelf<Window>(control);
				wnd.Closing -= WndClosing;
				wnd.Closing += WndClosing;

				var fs = new WndSettingsParams<T>()
				{
					Settings = settings,
					SettingName = settingName,
					Control = control
				};

				_Context[wnd] = fs;
			}
		}

		public abstract void SaveControlSettings(T control, string settingName, SettingsBag settings);


		protected abstract void LoadControlSettings(T control, string settingName, SettingsBag settings);


		protected void WndClosing(object sender, CancelEventArgs e)
		{
			Window wnd = sender as Window;
			wnd.Closing -= WndClosing;

			WndSettingsParams<T> fs = _Context[wnd];
			_Context.Remove(wnd);
			SaveControlSettings(fs.Control, fs.SettingName, fs.Settings);
		}


		protected class WndSettingsParams<TControl>
			where TControl : DependencyObject
		{
			public SettingsBag Settings { get; set; }
			public string SettingName { get; set; }
			public TControl Control { get; set; }
		}
	}
}

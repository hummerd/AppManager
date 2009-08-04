using System.IO;
using CommonLib;


namespace AppManager.Settings
{
	public class AMSettingLoader : FastSettingsLoader //XMLSettingsLoader<AppManagerSettings>
	{
		public MainWorkItem WorkItem { get; set; }

		#region ISettingProvider Members

		public override AppManagerSettings LoadSettings(string path)
		{
			string userSettingsDir = WorkItem.DataDir;
			userSettingsDir = Path.Combine(userSettingsDir, path);
			var result = base.LoadSettings(userSettingsDir);

			var heights = result.MianFormRowHeights;
			for (int i = 0; i < heights.Length; i++)
				if (heights[i] < 0.00001)
					heights[i] = 0.1;

			MathHelper.Normilize(heights, 1000);

			return result;
		}

		public override void SaveSettings(string path, AppManagerSettings settings)
		{
			string userSettingsDir = WorkItem.DataDir;
			userSettingsDir = Path.Combine(userSettingsDir, path);
			base.SaveSettings(userSettingsDir, settings);
		}

		#endregion
	}
}

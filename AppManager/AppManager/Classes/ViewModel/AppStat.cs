using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using AppManager.Entities;


namespace AppManager.Classes.ViewModel
{
	public class AppStat
	{
		public AppStat(AppInfo appInfo)
		{
			AppInfo = appInfo;
			RunCount = appInfo.RunHistory.Count;
		}

		public AppInfo AppInfo { get; set; }
		public long RunCount { get; set; }
		public double RealtiveRuns { get; set; }
	}

	public class AppStatCollection : ObservableCollection<AppStat>
	{
		public AppStatCollection(AppGroup apps)
		{
			foreach (var appType in apps.AppTypes)
				foreach (var item in appType.AppInfos)
				{
					Add(new AppStat(item));	
				}

			NormalizeRuns();
		}


		public void NormalizeRuns()
		{
			long maxRuns = 1;
			foreach (var item in this)
			{
				maxRuns = item.RunCount > maxRuns ? item.RunCount : maxRuns;
			}

			foreach (var item in this)
			{
				item.RealtiveRuns = item.RunCount * 100 / maxRuns;
			}
		}
	}

	public class GridSizeConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new GridLength((double)value, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class GridSizeRelativeConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new GridLength(100.0 - (double)value, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

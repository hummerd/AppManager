using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using AppManager.Windows;
using WinSh = IWshRuntimeLibrary;


namespace AppManager
{
	public class MainWindowController : ControllerBase
	{
		protected DispatcherTimer _SearchTimer = new DispatcherTimer();
		protected QuickSearch _QuickSearchWnd = null;


		public MainWindowController(MainWorkItem workItem)
			: base(workItem)
		{
			_SearchTimer.Interval = new TimeSpan(0, 0, 60);
			_SearchTimer.Tick += (s, e) => _QuickSearchWnd.Close();
		}


		public void RemoveAppType(AppType appType)
		{
			if (appType == null)
				return;

			_WorkItem.AppData.AppTypes.Remove(appType);
			_WorkItem.MainWindow.Init(false);
		}

		public void InsertAppType(AppType addAppType, AppType beforeAppType)
		{
			if (addAppType == null)
				return;

			if (beforeAppType == null)
			{
				_WorkItem.AppData.AppTypes.Insert(0, addAppType);
				return;
			}

			int ix = _WorkItem.AppData.AppTypes.IndexOf(beforeAppType);
			if (ix >= 0)
			{
				foreach (var item in addAppType.AppInfos)
					PrepareItem(item);

				_WorkItem.AppData.AppTypes.Insert(ix, addAppType);
			}

			_WorkItem.MainWindow.Init(false);
		}

		public void CreateDefaultType()
		{
			_WorkItem.AppData.AppTypes.Add(
				new AppType() { AppTypeName = Strings.APPLICATIONS }
				);

			_WorkItem.MainWindow.Init(false);
		}

		public void FindApp(string appNamePart)
		{
			if (_QuickSearchWnd == null)
			{
				_QuickSearchWnd = new QuickSearch();
				_QuickSearchWnd.Owner = _WorkItem.MainWindow;
				_QuickSearchWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				_QuickSearchWnd.SearchString = appNamePart;
				_QuickSearchWnd.SerachStringChanged += (s, e) => FindApp(_QuickSearchWnd.SearchString);
				_QuickSearchWnd.Closed += (s, e) => EndSearch();
				_QuickSearchWnd.ItemSelected += (s, e) => SearchSucceded();
				
				_QuickSearchWnd.FoundItems = _WorkItem.AppData.FindApps(appNamePart);
	
				_QuickSearchWnd.Show();
				_SearchTimer.IsEnabled = true;
			}
			else
			{
				_SearchTimer.Stop();
				_SearchTimer.Start();

				//var apps = _QuickSearchWnd.FoundItems as AppInfoCollection;
				var apps = _WorkItem.AppData.FindApps(appNamePart);

				if (apps != null)
					_QuickSearchWnd.FoundItems = apps;
			}
		}

		public void RenameItem(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			InputBox input = new InputBox(Strings.ENTER_APP_NAME);
			input.InputText = appInfo.AppName;
			input.Owner = _WorkItem.MainWindow;
			input.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			if (input.ShowDialog() ?? false)
			{
				appInfo.AppName = input.InputText;
			}
		}

		public void DeleteItem(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			if (MessageBox.Show(
				_WorkItem.MainWindow,
				string.Format(Strings.DEL_APP_QUEST, appInfo.AppName), 
				Strings.APP_TITLE,
				MessageBoxButton.YesNo) != MessageBoxResult.Yes)
				return;

			AppType appType = _WorkItem.AppData.FindAppType(appInfo);
			if (appType != null)
				appType.AppInfos.Remove(appInfo);
		}

		public void EditItem(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			_WorkItem.Commands.ManageApps.Execute(appInfo);
		}

		public void GoToAppFolder(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			appInfo.OpenFolder();
		}

		public void PrepareItem(AppInfo appInfo)
		{
			if (appInfo == null)
				return;

			appInfo.AppInfoID = _WorkItem.AppData.LastAppInfoID;
			_WorkItem.AppData.LastAppInfoID += 1;
			_WorkItem.AppData.RequestAppImage(appInfo);
		}

		public void AddFiles(AppType appType, string[] files)
		{
			if (appType == null)
				return;

			appType.AppInfos.AddRange(FindApps(files));
			_WorkItem.Commands.Save.Execute(null);
		}


		protected void SearchSucceded()
		{
			object si = _QuickSearchWnd.SelectedItem;
			_QuickSearchWnd.Close();
			_WorkItem.Commands.RunApp.Execute(si);
		}

		protected void EndSearch()
		{ 
			_SearchTimer.IsEnabled = false;
			_QuickSearchWnd = null;					
		}
	}
}

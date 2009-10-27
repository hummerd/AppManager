using System;
using System.Diagnostics;
using System.Reflection;
using CommonLib.Windows;
using UpdateLib;


namespace AppManager.Commands
{
	public class CheckVersion : CommandBase
	{
		protected SelfUpdate	_Updater;
		protected bool			_SilentUpdate = true;


		public CheckVersion(MainWorkItem workItem)
			: base(workItem)
		{ ; }


		public override void Execute(object parameter)
		{
			if (_Updater != null)
				return;

			_Updater = new SelfUpdate();
			_Updater.UpdateCompleted += (s, e) =>
				OnUpdateCompleted(e.SuccessfulCheck, e.HasNewVersion, e.Message);
			_Updater.NeedCloseApp += (s, e) =>
				_WorkItem.Commands.Quit.Execute(null);

			_WorkItem.UpdateRunning = true;
			_Updater.UpdateAppAsync(
				"AppManager",
				Strings.APP_TITLE,
				_WorkItem.AppPath,
				new string[] { Assembly.GetExecutingAssembly().Location },
				new string[] { Process.GetCurrentProcess().ProcessName },
				"http://dimanick.ru/MySoft/AppManager/Update"
				);
		}


		protected void OnUpdateCompleted(bool successfulCheck, bool hasNewVersion, string message)
		{
			_WorkItem.UpdateRunning = false;
			_WorkItem.NotifyUpdateCompleted();
			_Updater = null;

			if (_SilentUpdate)
			{
				_SilentUpdate = false;
				return;
			}

			var wnd = _WorkItem.FindActiveWindow();

			if (!successfulCheck)
			{
				ErrorBox.Show(
					Strings.APP_TITLE,
					Strings.UPDATE_CHECK_FAILED,
					message
					);

				return;
			}

			if (!hasNewVersion)
			{
				MsgBox.Show(
					wnd,
					Strings.APP_TITLE,
					String.Format(Strings.NO_NEW_VERSION, Strings.APP_TITLE),
					false);
			}
		}
	}
}

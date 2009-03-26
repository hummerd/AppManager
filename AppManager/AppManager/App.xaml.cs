using System.Windows;
using System;
using CommonLib.Windows;


namespace AppManager
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		//// Entry point method
		//[STAThread]
		//public static void Main()
		//{
		//    var app = new App();
		//    app.Run();
		//}


		protected MainWorkItem _WorkItem;


		public App()
		{
			//InitializeComponent();
			AppDomain.CurrentDomain.UnhandledException += UnhandledException;
		}


		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " OnStartup");

			_WorkItem = new MainWorkItem();
			MainWindow = _WorkItem.MainWindow;
			_WorkItem.Commands.Start.Execute(null);			
		}

		protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
		{
			base.OnSessionEnding(e);

			_WorkItem.Commands.Quit.Execute(null);
		}


		private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != null)
			{
				var exc = e.ExceptionObject as Exception;
				if (exc != null)
					ErrorBox.Show(Strings.ERROR, exc.Message, exc.ToString());
				else
					ErrorBox.Show(Strings.ERROR, Strings.ERROR_OCCUR, String.Empty);
			}
		}
	}
}

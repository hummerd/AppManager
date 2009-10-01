using System.Windows;
using System;
using CommonLib.Windows;
using System.Windows.Threading;


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
			InitializeComponent();
            DispatcherUnhandledException += delegate(object sender, DispatcherUnhandledExceptionEventArgs e)
            { 
                e.Handled = true; 
                HandleException(e.Exception); 
            };
		}


		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " OnStartup");

			_WorkItem = new MainWorkItem();
			MainWindow = _WorkItem.MainWindow;

			bool noupdate = false;
			if (e.Args.Length > 0 && e.Args[0] == "-noupdate")
				noupdate = true;

			_WorkItem.Commands.Start.Execute(noupdate);
		}

		protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
		{
			base.OnSessionEnding(e);

			_WorkItem.Commands.Quit.Execute(null);
		}

        protected void HandleException(Exception exc)
        {
            if (exc != null)
            {
                if (exc != null)
                    ErrorBox.Show(Strings.ERROR, exc);
                else
                    ErrorBox.Show(Strings.ERROR, Strings.ERROR_OCCUR, String.Empty);
            }            
        }
	}
}

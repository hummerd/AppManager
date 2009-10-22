using System.Threading;
using System.Windows.Threading;
using System;


namespace CommonLib.Application
{
	public class DispatcherHelper
	{
		public static void DoEvents()
		{
			DispatcherFrame frame = new DispatcherFrame();

			Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.Background,
				(SendOrPostCallback)delegate(object arg)
				{
					DispatcherFrame fr = arg as DispatcherFrame;
					fr.Continue = true;
				}, 
				frame);

			Dispatcher.PushFrame(frame);
		}

		public static void InvokeBackground(Delegate method)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, method);
			//Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, method);
		}

		public static void Invoke(Delegate method)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, method);
			//Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, method);
		}

		public static void PassExceptionOnUIThread(Exception exc)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(
				DispatcherPriority.Send,
				(SimpleMathod)delegate()
				{
					// THIS CODE RUNS BACK ON THE MAIN UI THREAD
					throw new Exception(CommStr.ERR_BACK_THREAD, exc);
				}
				);

			// NOTE - Application execution will only continue from this point
			//        onwards if the exception was handled on the main UI thread
			//        by Application.DispatcherUnhandledException
		}

		public static void Invoke(Delegate method, object arg)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, method, arg);
		}

		public static void Invoke(Delegate method, object arg0, object arg1)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, method, arg0, arg1);
		}

		public static void Invoke(Delegate method, object arg0, object arg1, object arg2)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, method, arg0, arg1, arg2);
		}
	}

	public delegate void SimpleMathod();
}

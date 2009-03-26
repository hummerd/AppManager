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

		public static void Invoke(Delegate method)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, method);
			//Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, method);
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
}

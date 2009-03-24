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
			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, method);
		}
	}
}

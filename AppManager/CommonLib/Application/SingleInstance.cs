using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows;
using System.Windows.Threading;


namespace CommonLib.Application
{
	public class SingleInstance
	{
		protected RemoteSingleInstance _Single;
		protected bool _FirstInstance = true;


		public SingleInstance(int port, bool sessionUnique)
		{
			if (sessionUnique)
				port += System.Diagnostics.Process.GetCurrentProcess().SessionId;

			try
			{
				InitFirstInstance(port);
				return;
			}
			catch(RemotingException)
			{
				_FirstInstance = false;
			}

			if (!_FirstInstance)
			{
				try
				{
					ActivateFirstInstance(port);
				}
				catch(Exception)
				{
					_FirstInstance = true;
				}
			}
		}


		public bool FirstInstance
		{
			get
			{
				return _FirstInstance;
			}
		}


		protected void InitFirstInstance(int port)
		{
			IpcChannel serverChannel = new IpcChannel("localhost:" + port);

			// Register the server channel.
			ChannelServices.RegisterChannel(
				 serverChannel, false);

			// Show the URIs associated with the channel.
			ChannelDataStore channelData = (ChannelDataStore)serverChannel.ChannelData;

			// Expose an object for remote calls.
			RemotingConfiguration.
				 RegisterWellKnownServiceType(
					  typeof(RemoteSingleInstance), "RemoteObject.rem",
					  WellKnownObjectMode.Singleton);
		}

		protected void ActivateFirstInstance(int port)
		{
			IpcChannel channel = new IpcChannel();

			// Register the channel.
			ChannelServices.RegisterChannel(channel, false);

			// Create an instance of the remote object.
			RemoteSingleInstance service = (RemoteSingleInstance)Activator.GetObject(
				typeof(RemoteSingleInstance),
				"ipc://localhost:" + port + "/RemoteObject.rem");
			service.ActivateApp();
		}
	}

	public class RemoteSingleInstance : MarshalByRefObject
	{
		protected delegate void SingleTask();


		public RemoteSingleInstance()
		{
		}


		public void ActivateApp()
		{
			DispatcherHelper.Invoke(new SingleTask(ActivateMainWnd));
			//System.Windows.Application.Current.Dispatcher.Invoke(
			//   DispatcherPriority.Normal, new SingleTask(ActivateMainWnd));
		}

		protected void ActivateMainWnd()
		{
			var wnd = System.Windows.Application.Current.MainWindow;

			bool top = wnd.Topmost;

			wnd.Show();
			wnd.Topmost = true;
			wnd.Focus();
			wnd.Topmost = top;
			wnd.Activate();
			wnd.InvalidateVisual();
		}
	}
}
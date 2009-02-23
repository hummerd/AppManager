using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows;


namespace CommonLib
{
	public class SingleInstance
	{
		protected RemoteSingleInstance _Single;
		protected bool _FirstInstance = true;


		public SingleInstance()
		{
			try
			{
				InitFirstInstance();
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
					ActivateFirstInstance();
				}
				catch
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


		protected void InitFirstInstance()
		{
			IpcChannel serverChannel = new IpcChannel("localhost:101632");

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

		protected void ActivateFirstInstance()
		{
			IpcChannel channel = new IpcChannel();

			// Register the channel.
			ChannelServices.RegisterChannel(channel, false);

			// Register as client for remote object.
			WellKnownClientTypeEntry remoteType =
				 new WellKnownClientTypeEntry(
					  typeof(RemoteSingleInstance),
					  "ipc://localhost:101632/RemoteObject.rem");
			RemotingConfiguration.RegisterWellKnownClientType(remoteType);

			// Create an instance of the remote object.
			RemoteSingleInstance service = (RemoteSingleInstance)Activator.GetObject(
				typeof(RemoteSingleInstance),
				"ipc://localhost:9090/RemoteObject.rem");
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
			Application.Current.Dispatcher.Invoke(new SingleTask(ActivateMainWnd));
		}

		protected void ActivateMainWnd()
		{ 
			var wnd = Application.Current.MainWindow;
			wnd.Show();
			wnd.Topmost = true;
			wnd.Focus();
			wnd.Topmost = false;
			wnd.Activate();
			wnd.InvalidateVisual();
		}
	}
}
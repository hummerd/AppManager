using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;


namespace CommonLib.Application
{
	public delegate void ActivateApp();


	public class SingleInstance
	{
		protected RemoteSingleInstance _Single;
		protected bool _FirstInstance = true;


		public SingleInstance(int port, bool sessionUnique, ActivateApp activator)
		{
			if (sessionUnique)
				port += System.Diagnostics.Process.GetCurrentProcess().SessionId;

			try
			{
				InitFirstInstance(port, activator);
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


		protected void InitFirstInstance(int port, ActivateApp activator)
		{
			IpcChannel serverChannel = new IpcChannel("localhost" + ":" + port);
			ChannelServices.RegisterChannel(serverChannel, false);

			//init and publish remote singletone object
			RemoteSingleInstance inst = new RemoteSingleInstance();
			inst.Activator = activator;
			RemotingServices.Marshal(inst, "RemoteObject.rem");
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
		public RemoteSingleInstance()
		{
		}


		public ActivateApp Activator
		{ get; set; }


		public void ActivateApp()
		{
			Activator();
		}
	}
}
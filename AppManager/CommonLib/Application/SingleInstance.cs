using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;


namespace CommonLib.Application
{
	public delegate void ActivateApp();


	public class SingleInstance2
	{
		protected Semaphore	_SyncObject;
		protected Thread	_SyncThread;
		protected bool		_FirstInstance = true;
		protected ActivateApp _Activator;


		public SingleInstance2(string appName, ActivateApp activator)
		{
			bool first;
			_Activator = activator;
			_SyncObject = new Semaphore(0, 1, appName, out first);
			_FirstInstance = first;

			if (first)
			{
				_SyncThread = new Thread(Activate);
				_SyncThread.IsBackground = true;
				_SyncThread.Start();
			}
			else
			{
				_SyncObject.Release();
			}
		}


		public bool FirstInstance
		{
			get
			{
				return _FirstInstance;
			}
		}

		protected void Activate()
		{ 
			while (true)
			{
				try
				{
					_SyncObject.WaitOne();
					_Activator();
					//_SyncObject.Release();
				}
				catch
				{ ; }
			}
			
		}
	}

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
			_Single = new RemoteSingleInstance();
			_Single.Activator = activator;
			RemotingServices.Marshal(_Single, "RemoteObject.rem");
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


		public override Object InitializeLifetimeService()
		{
			return null;
		}
	}
}
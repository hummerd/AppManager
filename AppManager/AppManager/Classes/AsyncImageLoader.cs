using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib;
using CommonLib.PInvoke;
using CommonLib.Shell;


namespace AppManager
{
	public class AsyncImageLoader
	{
		private static string[] IconOwnExt = new string[] { ".dll", ".exe" };


		protected DispatcherTimer _SearchTimer = new DispatcherTimer();

		protected object _RequestSync = new object();
		protected object _ResultSync = new object();
		protected Queue<AppInfo> _RequestedImages = new Queue<AppInfo>(100);
		protected Queue<Pair<AppInfo, Pair<System.Drawing.Icon, bool>>> _LoadedImages = new Queue<Pair<AppInfo, Pair<System.Drawing.Icon, bool>>>(100);
		protected Thread _LoadThread;


		public AsyncImageLoader()
		{
			_LoadThread = new Thread(ImageLoader);
			_LoadThread.SetApartmentState(ApartmentState.STA);
			_LoadThread.IsBackground = true;

			_SearchTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			_SearchTimer.Tick += (s, e) => SetImages();
			_SearchTimer.Start();
		}


		public void RequestImage(AppInfo app)
		{
			lock (_RequestSync)
				_RequestedImages.Enqueue(app);
		}
		
		public void StartLoad()
		{
			_LoadThread.Start();
		}


		protected void SetImages()
		{
			lock (_ResultSync)
			{
				while (_LoadedImages.Count > 0)
				{
					var pair = _LoadedImages.Dequeue();
					if (pair.Second.First != null)
					{
						pair.First.AppImage = Imaging.CreateBitmapSourceFromHIcon(
							pair.Second.First.Handle,
							Int32Rect.Empty,
							BitmapSizeOptions.FromEmptyOptions());

						if (pair.Second.Second)
							User32.DestroyIcon(pair.Second.First.Handle);

						pair.Second.First.Dispose();
					}
				}
			}
		}

		protected void ImageLoader(object param)
		{
			//Marshal.CoI

			while (true)
			{
				bool doLoad = true;

				lock (_RequestSync)
					doLoad = _RequestedImages.Count > 0;

				while (doLoad)
				{
					AppInfo app;
					lock (_RequestSync)
						app = _RequestedImages.Dequeue();

					bool managed;
					var src = LoadImage(app.ImagePath, out managed);

					lock (_ResultSync)
						_LoadedImages.Enqueue(
							new Pair<AppInfo, Pair<System.Drawing.Icon, bool>>() 
								{ First = app, Second = new Pair<System.Drawing.Icon, bool> 
									{ First = src, Second = managed } });

					lock (_RequestSync)
						doLoad = _RequestedImages.Count > 0;					
				}
				
				Thread.Sleep(1000);
			}
		}

		protected System.Drawing.Icon LoadImage(string path, out bool managed)
		{
			managed = true;

			try
			{
				path = Environment.ExpandEnvironmentVariables(path);

				if (!File.Exists(path))
					return null;
				
				if (PathHelper.IsPathUNC(path))
					return null;

				if (Array.IndexOf(IconOwnExt, Path.GetExtension(path)) >= 0)
					return System.Drawing.Icon.ExtractAssociatedIcon(path);
				else
				{
					managed = false;
					return ShFileInfo.ExtractIcon(path, true);
				}
			}
			catch
			{ ; }

			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib;


namespace AppManager
{
	public class AsyncImageLoader
	{
		protected DispatcherTimer _SearchTimer = new DispatcherTimer();

		protected object _RequestSync = new object();
		protected object _ResultSync = new object();
		protected Queue<AppInfo> _RequestedImages = new Queue<AppInfo>(100);
		protected Queue<Pair<AppInfo, System.Drawing.Icon>> _LoadedImages = new Queue<Pair<AppInfo, System.Drawing.Icon>>(100);
		protected Thread _LoadThread;


		public AsyncImageLoader()
		{
			_LoadThread = new Thread(ImageLoader);
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
					if (pair.Second != null)
						pair.First.AppImage = Imaging.CreateBitmapSourceFromHIcon(
							pair.Second.Handle,
							Int32Rect.Empty,
							BitmapSizeOptions.FromEmptyOptions());
				}
			}
		}

		protected void ImageLoader(object param)
		{
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

					var src = LoadImage(app.AppPath);

					lock (_ResultSync)
						_LoadedImages.Enqueue(
							new Pair<AppInfo, System.Drawing.Icon>() 
								{ First = app, Second = src });

					lock (_RequestSync)
						doLoad = _RequestedImages.Count > 0;					
				}
				
				Thread.Sleep(1000);
			}
		}

		protected System.Drawing.Icon LoadImage(string path)
		{
			try
			{
				if (!File.Exists(path))
					return null;
				
				if (PathHelper.IsPathUNC(path))
					return null;

				return System.Drawing.Icon.ExtractAssociatedIcon(path);
			}
			catch
			{ ; }

			return null;
		}
	}
}

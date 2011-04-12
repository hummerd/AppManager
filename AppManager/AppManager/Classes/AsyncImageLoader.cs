using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using AppManager.Entities;
using CommonLib;
using CommonLib.Application;
using CommonLib.PInvoke;
using CommonLib.Shell;


namespace AppManager
{
	public class AsyncImageLoader
	{
		private static string[] IconOwnExt = new string[] { ".ocx", ".dll", ".exe" };

		protected ManualResetEvent _WorkManager;

		protected object _RequestSync = new object();
		protected Queue<AppInfo> _RequestedImages = new Queue<AppInfo>(100);
		protected Thread _LoadThread;


		public AsyncImageLoader()
		{
		}


		public void RequestImage(AppInfo app)
		{
			lock (_RequestSync)
			{
				_RequestedImages.Enqueue(app);
				if (_WorkManager != null)
					_WorkManager.Set();
			}
		}

		public void StartLoad()
		{
			if (_LoadThread == null || !_LoadThread.IsAlive)
			{
				_WorkManager = new ManualResetEvent(true);
				_LoadThread = new Thread(ImageLoader);
				_LoadThread.SetApartmentState(ApartmentState.STA);
				_LoadThread.IsBackground = true;
			}

			_LoadThread.Start();
		}

		protected void SetImage(object arg)
		{
			var appImage = arg as Pair<AppInfo, System.Drawing.Icon>;

			if (appImage.Second != null)
			{
				appImage.First.AppImage = Imaging.CreateBitmapSourceFromHIcon(
					appImage.Second.Handle,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());

				User32.DestroyIcon(appImage.Second.Handle);
			}
		}

		#region Back thread

		protected void ImageLoader(object param)
		{
			try
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

						var src = LoadImage(app.LoadImagePath);

						var appImage = new Pair<AppInfo, System.Drawing.Icon>
						{
							First = app,
							Second = src,
						};
						DispatcherHelper.InvokeBackground((SimpleMathodArg)SetImage, appImage);
						
						lock (_RequestSync)
						{
							doLoad = _RequestedImages.Count > 0;
							if (!doLoad)
								_WorkManager.Reset();
						}
					}

					_WorkManager.WaitOne();
					//Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				// Dispatch the exception back to the main ui thread and reraise it
				DispatcherHelper.PassExceptionOnUIThread(ex);
			}
		}

		protected System.Drawing.Icon LoadImage(string path)
		{
			try
			{
				if (String.IsNullOrEmpty(path))
					return null;

				int ix = 0;
				string[] iconIx = path.Split(',');
				if (iconIx.Length > 1 && !String.IsNullOrEmpty(iconIx[1]))
					if (!int.TryParse(iconIx[1], out ix))
						ix = 0;

				path = Environment.ExpandEnvironmentVariables(iconIx[0]);

				if (!File.Exists(path))
					return null;

				if (PathHelper.IsPathUNC(path))
					return null;

				if (Array.IndexOf(IconOwnExt, Path.GetExtension(path)) >= 0)
				{
					return Shell32.ExtractIconEx(path, ix);
					//return System.Drawing.Icon.ExtractAssociatedIcon(path);
				}
				else
				{
					//managed = false;
					return ShFileInfo.ExtractIcon(path, true);
				}
			}
			catch
			{ ; }

			return null;
		}

		#endregion
	}
}

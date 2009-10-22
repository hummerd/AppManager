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
using CommonLib.Application;


namespace AppManager
{
	public class AsyncImageLoader
	{
		private static string[] IconOwnExt = new string[] { ".dll", ".exe" };

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

		protected void SetImage(AppInfo appInfo, Pair<System.Drawing.Icon, bool> loadResult)
		{
			if (loadResult.First != null)
			{
				appInfo.AppImage = Imaging.CreateBitmapSourceFromHIcon(
					loadResult.First.Handle,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());

				if (loadResult.Second)
					User32.DestroyIcon(loadResult.First.Handle);

				loadResult.First.Dispose();
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

						bool managed;
						var src = LoadImage(app.ImagePath, out managed);

						DispatcherHelper.InvokeBackground(
							(SimpleMathod)(
								() => SetImage(
									app, 
									new Pair<System.Drawing.Icon, bool> 
										{ First = src, Second = managed }))
							);

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

		#endregion
	}
}

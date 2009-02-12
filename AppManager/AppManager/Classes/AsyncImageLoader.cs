using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using AppManager.Common;


namespace AppManager
{
	public class AsyncImageLoader
	{
		//public event EventHandler<StrArrEventArgs> ImageLoaded;

		protected object _RequestSync = new object();
		protected object _ResultSync = new object();
		protected Queue<string> _RequestedImages = new Queue<string>(100);
		protected Dictionary<string, System.Drawing.Icon> _LoadedImages = new Dictionary<string, System.Drawing.Icon>(100);
		protected Thread _LoadThread;


		public AsyncImageLoader()
		{
			_LoadThread = new Thread(ImageLoader);
			_LoadThread.IsBackground = true;
		}


		public void RequestFile(string path)
		{
			lock (_RequestSync)
				_RequestedImages.Enqueue(path);
		}
		
		public void StartLoad()
		{
			_LoadThread.Start();
		}

		public bool HasImages()
		{
			lock (_ResultSync)
				return _LoadedImages.Count > 0;
		}

		public System.Drawing.Icon TryGetImage(string path)
		{
			System.Drawing.Icon result;
			bool exist = false;

			lock (_ResultSync)
			{
				exist = _LoadedImages.TryGetValue(path, out result);
				if (exist)
					_LoadedImages.Remove(path);
			}

			return exist ? result : null;
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
					string path;
					lock (_RequestSync)
						path = _RequestedImages.Dequeue();

					var src = LoadImage(path);

					lock (_ResultSync)
						_LoadedImages[path] = src;

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

				//return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
				//      ico.Handle,
				//      Int32Rect.Empty,
				//      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			}
			catch
			{ ; }

			return null;
		}
	}
}

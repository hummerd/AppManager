using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using AppManager.EntityCollection;
using CommonLib;
using CommonLib.IO;
using System.Collections.Generic;


namespace AppManager.Entities
{
	public class AppInfoCollection : EntityCollection<AppInfo>
	{
		public AppInfoCollection()
		{

		}

		public AppInfoCollection(IList collection)
			: base(collection)
		{

		}
	}

	public class AppInfo : EntityBase<AppInfo>
	{
		private static BitmapSource _BlankImage;
		private static BitmapSource _FolderImage;

		private static BitmapSource GetBlankImage()
		{
			if (_BlankImage == null)
			{
				//using (var bmp = AppManager.Properties.Resources.Window)
				//{
				//   _BlankImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
				//      bmp.GetHbitmap(),
				//      IntPtr.Zero,
				//      Int32Rect.Empty,
				//      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
				//}
				//in all other cases we've got size {5, 5} instead of {16, 16}

				//Uri pngSrc = new Uri(@"..\..\Resources\Window.png", UriKind.RelativeOrAbsolute);

				//_BlankImage = new BitmapImage();
				//_BlankImage.BeginInit();
				//_BlankImage.DecodePixelWidth = 48;
				//_BlankImage.UriSource = pngSrc;
				//_BlankImage.EndInit();

				//PngBitmapDecoder decoder = new PngBitmapDecoder(pngSrc, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
				//_BlankImage = decoder.Frames[0];

				//_BlankImage = new CroppedBitmap(
				//   new BitmapImage(
				//      new Uri(@"..\..\Resources\Window.png", UriKind.RelativeOrAbsolute)), 
				//   new Int32Rect(0, 0, 16, 16));


				//The deal was in images DPI
				//http://genesisconduit.wordpress.com/2008/07/05/wpf-images-and-dpi-independence/
				_BlankImage = new BitmapImage(
					new Uri(@"pack://application:,,/Resources/Window.png"));
			}

			return _BlankImage;
		}
		private static BitmapSource GetFolderImage()
		{
			if (_FolderImage == null)
			{
				//using (var bmp = AppManager.Properties.Resources.folder)
				//{
				//_FolderImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
				//   bmp.GetHbitmap(),
				//   IntPtr.Zero,
				//   Int32Rect.Empty,
				//   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
				//}

				_FolderImage = new BitmapImage(
					new Uri(@"pack://application:,,/Resources/folder.png"));
			}

			return _FolderImage;
		}


		public event EventHandler NeedImage;
		
		
		protected BitmapSource _AppImage;

		protected string _ExecPath;
		protected string _AppName;
		protected string _ImagePath;
		protected AppRunInfoCollection _RunHostory;


		public AppInfo()
		{
			_RunHostory = new AppRunInfoCollection();
		}


		public string AppName
		{
			get
			{
				return _AppName;
			}
			set
			{
				_AppName = value;
				OnPropertyChanged("AppName");
			}
		}

		public string ExecPath
		{ 
			get { return _ExecPath; } 
			set 
			{ 
				_ExecPath = (value ?? String.Empty).Trim();
				
				if (!HasImagePath)
				{
					LoadImage();
					OnNeedImage();
					OnPropertyChanged("AppImage");
				}

				OnPropertyChanged("ExecPath");
			}
		}

		public string AppPath
		{
			get
			{
				string args;
				return PathHelper.GetFilePathFromExecPath(ExecPath, out args);
			}
			set
			{
				SetExecPath(value, AppArgs);
			}
		}

		public string AppArgs
		{
			get
			{
				string args;
				PathHelper.GetFilePathFromExecPath(ExecPath, out args);
				return args;
			}
			set
			{
				SetExecPath(AppPath, value);
			}
		}

		public bool HasImagePath
		{
			get
			{
				return !String.IsNullOrEmpty(_ImagePath);
			}
		}

		public string ImagePath
		{
			get
			{
				return _ImagePath;
			}
			set
			{
				string newVal = Environment.ExpandEnvironmentVariables(value ?? String.Empty);
				if (LnkHelper.CompareIconPath(newVal, AppPath))
					newVal = String.Empty;

				if (!String.IsNullOrEmpty(newVal) || !String.IsNullOrEmpty(_ImagePath))
				{
					_ImagePath = newVal;
					LoadImage();
					OnNeedImage();
					OnPropertyChanged("ImagePath");
					OnPropertyChanged("AppImage");
				}
			}
		}

		public string LoadImagePath
		{
			get
			{
				if (String.IsNullOrEmpty(_ImagePath))
					return AppPath;
				else
					return ImagePath;
			}
			set
			{
				ImagePath = value;
			}
		}

		public BitmapSource AppImage
		{ 
			get 
			{
				if (_AppImage == null || _AppImage.Width == 0)
					return GetBlankImage();

				return _AppImage; 
			} 
			set 
			{ 
				_AppImage = value;
				OnPropertyChanged("AppImage");
			} 
		}

		public string AppPathInfo
		{
			get
			{
				return AppName + Environment.NewLine + AppPath;
			}
		}

		public AppRunInfoCollection RunHistory
		{
			get { return _RunHostory; }
		}


		public void OpenFolder()
		{
			string appPath = AppPath;
			string dir = Path.GetDirectoryName(appPath);

			if (String.IsNullOrEmpty(appPath))
				return;

			using (Process p = new Process())
			{
				if (Directory.Exists(appPath))
				{
					p.StartInfo.FileName = appPath;
					p.StartInfo.WorkingDirectory = appPath;
				}
				else if (File.Exists(appPath))
				{
					p.StartInfo.Arguments = "/select, " + appPath;
					p.StartInfo.FileName = "explorer";
				}
				else if (Directory.Exists(dir))
				{
					p.StartInfo.FileName = dir;
					p.StartInfo.WorkingDirectory = dir;
				}
				else
				{
					dir = PathHelper.GetExistingPath(dir);
					p.StartInfo.FileName = dir;
					p.StartInfo.WorkingDirectory = dir;
				}

				if (!String.IsNullOrEmpty(p.StartInfo.FileName))
					p.Start();
			}
		}

		public void RunApp()
		{
			if (File.Exists(AppPath) || Directory.Exists(AppPath))
			{
				Process p = new Process();
				p.StartInfo.FileName = AppPath;
				p.StartInfo.WorkingDirectory = Path.GetDirectoryName(AppPath);
				p.StartInfo.Arguments = AppArgs;
				p.Start();
			}
		}

		public void RequestAppImage()
		{
			OnNeedImage();
		}

		public bool SetAutoAppName()
		{
			if (String.IsNullOrEmpty(AppPath))
				return false;

			if (AppName == Strings.NEW_APP)
			{
				string name = Path.GetFileNameWithoutExtension(AppPath).ToLower();
				if (String.IsNullOrEmpty(name))
					name = AppPath;

				if (String.IsNullOrEmpty(name))
					return false;

				AppName = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
				return true;
			}

			return false;
		}

		public void SetExecPath(string appPath, string args)
		{
			if (String.IsNullOrEmpty(appPath))
				return;

			if (args == null)
				args = String.Empty;

			ExecPath = "\"" + appPath.Trim() + "\"" + " " + args.Trim();
		}


		public override string ToString()
		{
			return AppName;
		}

		public bool EqualsByExecPath(AppInfo appInfo)
		{
			return
				string.Equals(appInfo.ExecPath, ExecPath, StringComparison.CurrentCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			var ai = obj as AppInfo;
			if (ai == null)
				return false;

			return ID == ai.ID;
		}

		public override int GetHashCode()
		{
			return ID;
		}


		protected virtual void OnNeedImage()
		{
			if (NeedImage != null && File.Exists(AppPath))
				NeedImage(this, EventArgs.Empty);
		}

		protected void LoadImage()
		{
			if (String.IsNullOrEmpty(LoadImagePath))
			{
				AppImage = GetBlankImage();
				return;
			}

			BitmapSource src = null;

			string appPath = LoadImagePath;

			if (PathHelper.IsLikeDrive(appPath) ||
				 Directory.Exists(appPath))
			{
				src = GetFolderImage();
			}

			_AppImage = src;
		}


		protected override void MergeEntity(AppInfo source, bool clone)
		{
			base.MergeEntity(source, clone);

			if (AppName != source.AppName)
				AppName = source.AppName;

			if (ImagePath != source.ImagePath)
				ImagePath = source.ImagePath;

			if (ExecPath != source.ExecPath)
				ExecPath = source.ExecPath;
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using AppManager.EntityCollection;
using CommonLib;


namespace AppManager
{
	public class AppInfoCollection : EntityCollection<AppInfo>
	{
		public AppInfoCollection()
		{

		}

		public AppInfoCollection(IEnumerable<AppInfo> collection)
			: base(collection)
		{

		}


		public void AddRange(IEnumerable<AppInfo> items)
		{
			foreach (var item in items)
				Add(item);
		}
	}

	[Serializable]
	public class AppInfo : IClonableEntity<AppInfo>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler NeedImage;


		[XmlIgnore]
		protected BitmapSource _BlankImage;
		[XmlIgnore]
		protected BitmapSource _FolderImage;
		[XmlIgnore]
		protected BitmapSource _AppImage;

		protected int _AppInfoID;
		protected string _ExecPath;
		protected string _AppName;


		public AppInfo()
		{
		
		}


		public int AppInfoID
		{
			get
			{
				return _AppInfoID;
			}
			set
			{
				_AppInfoID = value;
			}
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
				OnPropertyChanged(new PropertyChangedEventArgs("AppName"));
			}
		}

		public string ExecPath
		{ 
			get { return _ExecPath; } 
			set 
			{ 
				_ExecPath = (value ?? String.Empty).Trim();
				LoadImage();
				OnNeedImage();
				OnPropertyChanged(new PropertyChangedEventArgs("ExecPath"));
				OnPropertyChanged(new PropertyChangedEventArgs("AppImage"));
			}
		}

		[XmlIgnore]
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

		[XmlIgnore]
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

		//public string ImagePath { get; set; }
		[XmlIgnore]
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
				OnPropertyChanged(new PropertyChangedEventArgs("AppImage"));
			} 
		}

		[XmlIgnore]
		public string AppPathInfo
		{
			get
			{
				return AppName + Environment.NewLine + AppPath;
			}
		}


		public void OpenFolder()
		{
			string dir = Path.GetDirectoryName(AppPath);

			if (String.IsNullOrEmpty(dir))
				return;

			if (Directory.Exists(dir))
			{
				Process p = new Process();
				p.StartInfo.FileName = dir;
				p.StartInfo.WorkingDirectory = dir;
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


		protected virtual void OnNeedImage()
		{
			if (NeedImage != null)
				NeedImage(this, EventArgs.Empty);
		}

		protected void LoadImage()
		{
			if (String.IsNullOrEmpty(ExecPath))
			{
				AppImage = GetBlankImage();
				return;
			}

			BitmapSource src = null;

			//if (File.Exists(AppPath))
			//{
			//   if (PathHelper.IsPathUNC(AppPath))
			//   {
			//      AppImage = GetBlankImage();
			//      return;
			//   }

			//   System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(AppPath);

			//   src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
			//      ico.Handle,
			//      Int32Rect.Empty,
			//      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			//} else
			string appPath = AppPath;

			if (PathHelper.IsLikeDrive(appPath) || 
				 Directory.Exists(appPath))
			{
				src = GetFolderImage();
			}

			_AppImage = src;
		}

		protected BitmapSource GetBlankImage()
		{
			if (_BlankImage == null)
			{
				_BlankImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					AppManager.Properties.Resources.Window.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

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

				//_BlankImage = new BitmapImage(
				//      new Uri(@"..\..\Resources\Window.png", UriKind.RelativeOrAbsolute));
			}

			return _BlankImage;
		}

		protected BitmapSource GetFolderImage()
		{
			if (_FolderImage == null)
			{
				_FolderImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					AppManager.Properties.Resources.folder.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			}

			return _FolderImage;
		}

		#region IClonableEntity<AppInfo> Members

		public AppInfo CloneSource
		{
			get;
			set;
		}

		public AppInfo CloneEntity()
		{
			AppInfo clone = new AppInfo();
			clone.AppInfoID = AppInfoID;
			clone.AppName = AppName;
			clone.ExecPath = ExecPath;
			clone.CloneSource = this;

			return clone;
		}

		public void MergeEntity(AppInfo source)
		{
			if (AppName != source.AppName)
				AppName = source.AppName;

			if (ExecPath != source.ExecPath)
				ExecPath = source.ExecPath;

			//EntityMerger.MergeEntity(source, this);
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return CloneEntity();
		}

		#endregion

		public override string ToString()
		{
			return AppName;
		}
		
		public override bool Equals(object obj)
		{
			return AppInfoID == ((AppInfo)obj).AppInfoID;
		}

		public override int GetHashCode()
		{
			return AppInfoID;
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}

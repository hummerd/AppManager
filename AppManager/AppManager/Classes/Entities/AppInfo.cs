using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Windows;
using AppManager.EntityCollection;
using System.ComponentModel;
using System.IO;


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
	}

	[Serializable]
	public class AppInfo : IClonableEntity<AppInfo>, INotifyPropertyChanged
	{
		public static string DefaultAppName = "Новое приложение";

		//public static string GetNameFromPath(string path)
		//{
		//   string name = Path.GetFileNameWithoutExtension(path).ToLower();
		//   return name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
		//}

		//public static string GetFilePathFromExecPath(string path)
		//{
		//   string args;
		//   return GetFilePathFromExecPath(path, out args);
		//}

		//public static string GetFilePathFromExecPath(string path, out string args)
		//{
		//   args = String.Empty;

		//   if (path != null)
		//      path = path.Trim();

		//   if (String.IsNullOrEmpty(path))
		//      return String.Empty;

		//   int lix = path.LastIndexOf('"');
		//   string filePath = path.Substring(0, lix);
		//   filePath = filePath.Trim();
		//   filePath = filePath.Trim('\"');
		//   filePath = filePath.Trim();
						
		//   if (path.Length > lix + 1)
		//   {
		//      args = path.Substring(lix + 1, path.Length - lix - 1);
		//      args = args.Trim();
		//   }

		//   return filePath;
		//}


		public event PropertyChangedEventHandler PropertyChanged;

		[XmlIgnore]
		protected BitmapSource _BlankImage;
		[XmlIgnore]
		protected BitmapSource _AppImage;

		protected string _ExecPath;
		protected string _AppName;


		public AppInfo()
		{
		
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
				_ExecPath = value; 
				LoadImage();
				OnPropertyChanged(new PropertyChangedEventArgs("ExecPath"));
				OnPropertyChanged(new PropertyChangedEventArgs("AppImage"));
			}
		}

		public string AppPath
		{
			get
			{
				string args;
				return GetFilePathFromExecPath(out args);
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
				GetFilePathFromExecPath(out args);
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


		public bool SetAutoAppName()
		{
			if (String.IsNullOrEmpty(AppPath))
				return false;

			if (AppName != DefaultAppName)
			{
				string name = Path.GetFileNameWithoutExtension(AppPath).ToLower();
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



		protected string GetFilePathFromExecPath(out string args)
		{
			args = String.Empty;
			string path = ExecPath;

			if (path != null)
				path = path.Trim();

			if (String.IsNullOrEmpty(path))
				return String.Empty;

			int lix = path.LastIndexOf('"');
			string filePath;
			if (lix >= 0)
			{
				filePath = path.Substring(0, lix);
				filePath = filePath.Trim();
				filePath = filePath.Trim('\"');
				filePath = filePath.Trim();
			}
			else
				filePath = path;
						
			if (lix >= 0 && path.Length > lix + 1)
			{
				args = path.Substring(lix + 1, path.Length - lix - 1);
				args = args.Trim();
			}

			return filePath;
		}

		protected BitmapSource GetBlankImage()
		{
			if (_BlankImage == null)
			{
				_BlankImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
					AppManager.Properties.Resources.Window1.Handle,
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

		protected void LoadImage()
		{
			if (String.IsNullOrEmpty(ExecPath))
			{
				AppImage = GetBlankImage();
				return;
			}

			System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(AppPath);

			BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
				ico.Handle,
				Int32Rect.Empty,
				System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

			_AppImage = src;
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


		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}

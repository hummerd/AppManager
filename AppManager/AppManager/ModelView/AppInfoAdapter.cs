using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using AppManager.Entities;
using System.Windows.Media.Imaging;


namespace AppManager.UIAdapter
{
	public class AppInfoAdapterCollection : List<AppInfoAdapter>
	{
		public AppInfoAdapter FindByNameStart(string start, int greaterThen)
		{
			if (greaterThen >= Count - 1)
				greaterThen = 0;
			else
				greaterThen++;

			//look in tale
			for (int i = greaterThen; i < Count; i++)
			{
				if (this[i].AppName.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
					return this[i];
			}

			//if not found in tale look in head
			for (int i = 0; i < greaterThen; i++)
			{
				if (this[i].AppName.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
					return this[i];
			}

			return null;
		}
	}


	public class AppInfoAdapter : INotifyPropertyChanged
	{
		protected AppInfo _Source;
		protected bool _Checked = true;


		public AppInfoAdapter(AppInfo source)
		{
			_Source = source;
			_Source.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
		}


		public AppInfo App { get { return _Source; } }

		public string AppName { get { return _Source.AppName; } set { _Source.AppName = value; } }
		public string ExecPath { get { return _Source.ExecPath; } set { } }
		public BitmapSource AppImage { get { return _Source.AppImage; } }

		public bool Checked
		{
			get { return _Checked; }
			set { _Checked = value; OnPropertyChanged("Checked"); }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		#endregion

		public override string ToString()
		{
			return _Source.ToString();
		}
	}
}

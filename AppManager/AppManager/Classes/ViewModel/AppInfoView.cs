using System;
using System.Collections.Generic;
using System.Text;
using AppManager.Entities;
using CommonLib;
using System.ComponentModel;


namespace AppManager.Classes.ViewModel
{
	public class AppInfoView : ISourceReference<AppInfo>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		protected bool m_ShowTitle;


		public AppInfo Source
		{
			get;
			set;
		}

		public bool ShowTitle
		{ 
			get
			{
				return m_ShowTitle;
			}
			set
			{
				m_ShowTitle = value;
				OnPropertyChanged("ShowTitle");
			}
		}


		protected virtual void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
			{
				var ea = new PropertyChangedEventArgs(propName);
				PropertyChanged(this, ea);
			}
		}
	}
}

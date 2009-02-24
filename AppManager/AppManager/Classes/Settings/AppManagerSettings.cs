using System.ComponentModel;


namespace AppManager.Settings
{
	public class AppManagerSettings : INotifyPropertyChanged
	{
		protected bool _AlwaysOnTop;
		protected bool _StartMinimized;


		public AppManagerSettings()
		{
			MainFormSett = WndSettings.Empty;
		}


		public double[] MianFormRowHeights
		{ get; set; }

		public WndSettings MainFormSett
		{ get; set; }

		public bool StartMinimized
		{
			get
			{
				return _StartMinimized;
			}
			set
			{
				_StartMinimized = value;
				OnPropertyChanged("StartMinimized");
			}
		}

		public bool AlwaysOnTop
		{
			get
			{
				return _AlwaysOnTop;
			}
			set
			{
				_AlwaysOnTop = value;
				OnPropertyChanged("AlwaysOnTop");
			}
		}


		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}

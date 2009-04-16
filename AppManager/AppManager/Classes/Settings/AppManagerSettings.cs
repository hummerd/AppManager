using System.ComponentModel;


namespace AppManager.Settings
{
	public class AppManagerSettings : INotifyPropertyChanged
	{
		protected bool _AlwaysOnTop;
		protected bool _StartMinimized;
		protected bool _EnableActivationPanel;
		protected bool _UseShortActivationPanel;


		public AppManagerSettings()
		{
			MainFormSett = WndSettings.Empty;
			_EnableActivationPanel = true;
			_UseShortActivationPanel = true;
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

		public bool EnableActivationPanel
		{
			get
			{
				return _EnableActivationPanel;
			}
			set
			{
				_EnableActivationPanel = value;
				OnPropertyChanged("EnableActivationPanel");
			}
		}

		public bool UseShortActivationPanel
		{
			get
			{
				return _UseShortActivationPanel;
			}
			set
			{
				_UseShortActivationPanel = value;
				OnPropertyChanged("UseShortActivationPanel");
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

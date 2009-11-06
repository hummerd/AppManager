using System;
using System.ComponentModel;
using System.Windows.Media;


namespace AppManager.Settings
{
	[Serializable]
	public class AppManagerSettings : INotifyPropertyChanged
	{
		protected bool _TransparentActivationPanel;
		protected Color _ActivationPanelColor;
		protected bool _AlwaysOnTop;
		protected bool _StartMinimized;
		protected bool _EnableActivationPanel;
		protected bool _UseShortActivationPanel;
		protected bool _CheckNewVersionAtStartUp;


		public AppManagerSettings()
		{
			MainFormSett = WndSettings.Empty;
			MianFormRowHeights = new double[0];
			_EnableActivationPanel = true;
			_UseShortActivationPanel = true;
			_CheckNewVersionAtStartUp = true;
			_ActivationPanelColor = Colors.Aqua;
			// if version greater then xp
			_TransparentActivationPanel = Environment.OSVersion.Version.Major > 5;

			NotifyPropertyChanged = true;
		}


		public bool NotifyPropertyChanged
		{ get; set; }


		public bool TransparentActivationPanel
		{
			get
			{
				return _TransparentActivationPanel;
			}
			set
			{
				_TransparentActivationPanel = value;
				OnPropertyChanged("TransparentActivationPanel");
			}
		}

		public Color ActivationPanelColor
		{
			get
			{
				return _ActivationPanelColor;
			}
			set
			{
				_ActivationPanelColor = value;
				OnPropertyChanged("ActivationPanelColor");
			}
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

		public bool CheckNewVersionAtStartUp
		{
			get
			{
				return _CheckNewVersionAtStartUp;
			}
			set
			{
				_CheckNewVersionAtStartUp = value;
				OnPropertyChanged("CheckNewVersionAtStartUp");
			}
		}


		public void NotifyAllPropertyChanged()
		{
			OnPropertyChanged("All");
		}


		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (NotifyPropertyChanged && PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}

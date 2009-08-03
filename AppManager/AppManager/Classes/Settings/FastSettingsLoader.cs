using System;
using System.Collections.Generic;
using System.Text;
using AppManager.Settings;
using System.Xml;


namespace AppManager.Settings
{
	public class FastSettingsLoader : ISettingProvider<AppManagerSettings>
	{
		#region ISettingProvider<AppManagerSettings> Members

		public virtual AppManagerSettings LoadSettings(string path)
		{
			AppManagerSettings result = new AppManagerSettings();
			using (XmlReader reader = XmlReader.Create(path))
			{
				reader.Read();
				reader.ReadStartElement("AppManagerSettings");
				reader.ReadStartElement("MianFormRowHeights");

				List<double> rowHeights = new List<double>();
				while (reader.IsStartElement())
				{
					reader.ReadStartElement("double");
					rowHeights.Add(reader.ReadContentAsDouble());
					reader.ReadEndElement();
				}
				reader.ReadEndElement();
				result.MianFormRowHeights = rowHeights.ToArray();

				reader.ReadStartElement("MainFormSett");
					reader.ReadStartElement("Location");

						WndSettings wndSett;
						reader.ReadStartElement("X");
						wndSett.Location.X = reader.ReadContentAsDouble();
						reader.ReadEndElement();

						reader.ReadStartElement("Y");
						wndSett.Location.Y = reader.ReadContentAsDouble();
						reader.ReadEndElement();

					reader.ReadEndElement(); //Location

					reader.ReadStartElement("Size");

						reader.ReadStartElement("Width");
						wndSett.Size.Width = reader.ReadContentAsDouble();
						reader.ReadEndElement();

						reader.ReadStartElement("Height");
						wndSett.Size.Height = reader.ReadContentAsDouble();
						reader.ReadEndElement();

					reader.ReadEndElement(); //Size
				reader.ReadEndElement(); //MainFormSett

				result.MainFormSett = wndSett;

				reader.ReadStartElement("StartMinimized");
				result.StartMinimized = reader.ReadContentAsBoolean();
				reader.ReadEndElement();

				reader.ReadStartElement("AlwaysOnTop");
				result.AlwaysOnTop = reader.ReadContentAsBoolean();
				reader.ReadEndElement();

				reader.ReadStartElement("EnableActivationPanel");
				result.EnableActivationPanel = reader.ReadContentAsBoolean();
				reader.ReadEndElement();

				reader.ReadStartElement("UseShortActivationPanel");
				result.UseShortActivationPanel = reader.ReadContentAsBoolean();
				reader.ReadEndElement();

				reader.ReadEndElement();
			}

			return result;
		}

		public virtual void SaveSettings(string path, AppManagerSettings settings)
		{
			
		}

		#endregion

		protected void LoadRowHeight()
		{ 
			
		}
	}
}

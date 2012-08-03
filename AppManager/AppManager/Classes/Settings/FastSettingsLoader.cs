using System.Collections.Generic;
using System.Xml;
using CommonLib;


namespace AppManager.Settings
{
	public class FastSettingsLoader : ISettingProvider<AppManagerSettings>
	{
		#region ISettingProvider<AppManagerSettings> Members

		public virtual AppManagerSettings LoadSettings(string path)
		{
			AppManagerSettings result = null;
			bool readNew = false;
			try
			{
				result = LoadSettings2(path);
				readNew = true;
			}
			catch
			{
				result = null;
			}

			if (!readNew)
				try
				{
					result = LoadSettings1(path);
				}
				catch
				{
					result = null;
				}

			return result;
		}

		public virtual void SaveSettings(string path, AppManagerSettings settings)
		{
			XmlWriterSettings sett = new XmlWriterSettings();
			sett.Indent = true;
			using (var writer = XmlWriter.Create(path, sett))
			{
				writer.WriteStartElement("AppManagerSettings");

					writer.WriteStartElement("MianFormRowHeights");

					foreach (var item in settings.MianFormRowHeights)
					{
						writer.WriteStartElement("Height");
						writer.WriteValue(item);
						writer.WriteEndElement();
					}
				
					writer.WriteEndElement();


					writer.WriteStartElement("MainFormPos");

						writer.WriteStartAttribute("Left");
						writer.WriteValue(settings.MainFormSett.Location.X);
						writer.WriteEndAttribute();

						writer.WriteStartAttribute("Top");
						writer.WriteValue(settings.MainFormSett.Location.Y);
						writer.WriteEndAttribute();

						writer.WriteStartAttribute("Widht");
						writer.WriteValue(settings.MainFormSett.Size.Width);
						writer.WriteEndAttribute();

						writer.WriteStartAttribute("Height");
						writer.WriteValue(settings.MainFormSett.Size.Height);
						writer.WriteEndAttribute();

					writer.WriteEndElement();

					writer.WriteStartElement("StartMinimized");
					writer.WriteValue(settings.StartMinimized);
					writer.WriteEndElement();

					writer.WriteStartElement("AlwaysOnTop");
					writer.WriteValue(settings.AlwaysOnTop);
					writer.WriteEndElement();

					writer.WriteStartElement("EnableActivationPanel");
					writer.WriteValue(settings.EnableActivationPanel);
					writer.WriteEndElement();

					writer.WriteStartElement("UseShortActivationPanel");
					writer.WriteValue(settings.UseShortActivationPanel);
					writer.WriteEndElement();

					writer.WriteStartElement("CheckNewVersionAtStartUp");
					writer.WriteValue(settings.CheckNewVersionAtStartUp);
					writer.WriteEndElement();

					writer.WriteStartElement("ActivationPanelColor");
					writer.WriteValue(UIHelper.ToARGB(settings.ActivationPanelColor));
					writer.WriteEndElement();

					writer.WriteStartElement("TransparentActivationPanel");
					writer.WriteValue(settings.TransparentActivationPanel);
					writer.WriteEndElement();

					writer.WriteStartElement("ShowAppTitles");
					writer.WriteValue(settings.ShowAppTitles);
					writer.WriteEndElement();

                    writer.WriteStartElement("StatisticPeriod");
                    writer.WriteValue(settings.StatisticPeriod);
                    writer.WriteEndElement();

				writer.WriteEndElement();
			}
		}

		#endregion


		protected virtual AppManagerSettings LoadSettings1(string path)
		{
			AppManagerSettings result = new AppManagerSettings();

			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			using (XmlReader reader = XmlReader.Create(path, sett))
			{
				reader.ReadStartElement("AppManagerSettings");
				reader.ReadStartElement("MianFormRowHeights");

				List<double> rowHeights = new List<double>();

				if (reader.Name == "double")
				{
				//var sub = reader.ReadSubtree();
				//if (reader.ReadToDescendant("double"))
				//{
					while (reader.IsStartElement())
					{
						reader.ReadStartElement("double");
						rowHeights.Add(reader.ReadContentAsDouble());
						reader.ReadEndElement();
					}
					reader.ReadEndElement();
				}
				result.MianFormRowHeights = rowHeights.ToArray();

				reader.ReadStartElement("MainFormSett");
					reader.ReadStartElement("Location");

						WndSettings wndSett = new WndSettings();
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

		protected virtual AppManagerSettings LoadSettings2(string path)
		{
			AppManagerSettings result = new AppManagerSettings();

			XmlReaderSettings sett = new XmlReaderSettings();
			sett.IgnoreWhitespace = true;
			using (XmlReader reader = XmlReader.Create(path, sett))
			{
				reader.ReadStartElement("AppManagerSettings");
				reader.ReadStartElement("MianFormRowHeights");

				List<double> rowHeights = new List<double>();

				if (reader.Name == "Height")
				{
					while (reader.IsStartElement())
					{
						reader.ReadStartElement("Height");
						rowHeights.Add(reader.ReadContentAsDouble());
						reader.ReadEndElement();
					}
					reader.ReadEndElement();
				}
				result.MianFormRowHeights = rowHeights.ToArray();

				//reader.ReadToFollowing("MainFormSett");

					WndSettings wndSett = new WndSettings();
					reader.MoveToAttribute("Left");
					wndSett.Location.X = reader.ReadContentAsDouble();

					reader.MoveToAttribute("Top");
					wndSett.Location.Y = reader.ReadContentAsDouble();

					reader.MoveToAttribute("Widht");
					wndSett.Size.Width = reader.ReadContentAsDouble();
				
					reader.MoveToAttribute("Height");
					wndSett.Size.Height = reader.ReadContentAsDouble();

					result.MainFormSett = wndSett;

				reader.Read();

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

				if (reader.NodeType == XmlNodeType.Element)
				{
					reader.ReadStartElement("CheckNewVersionAtStartUp");
					result.CheckNewVersionAtStartUp = reader.ReadContentAsBoolean();
					reader.ReadEndElement();
				}

				if (reader.NodeType == XmlNodeType.Element)
				{
					reader.ReadStartElement("ActivationPanelColor");
					result.ActivationPanelColor = UIHelper.FromARGB(reader.ReadContentAsInt());
					reader.ReadEndElement();
				}

				if (reader.NodeType == XmlNodeType.Element)
				{
					reader.ReadStartElement("TransparentActivationPanel");
					result.TransparentActivationPanel = reader.ReadContentAsBoolean();
					reader.ReadEndElement();
				}

				if (reader.NodeType == XmlNodeType.Element)
				{
					reader.ReadStartElement("ShowAppTitles");
					result.ShowAppTitles = reader.ReadContentAsBoolean();
					reader.ReadEndElement();
				}

                if (reader.NodeType == XmlNodeType.Element)
                {
                    reader.ReadStartElement("StatisticPeriod");
                    result.StatisticPeriod = reader.ReadContentAsInt();
                    reader.ReadEndElement();
                }

				reader.ReadEndElement();
			}

			return result;
		}
	}
}

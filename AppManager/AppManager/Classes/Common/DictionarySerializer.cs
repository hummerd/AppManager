using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;


namespace AppManager.Classes.Common
{
	[XmlRoot("dictionary")]
	public class DictionarySerializer<TKey, TValue> : IXmlSerializable
	{
		[XmlIgnore]
		protected Dictionary<TKey, TValue> _Source;


		public DictionarySerializer()
		{
			_Source = new Dictionary<TKey, TValue>();
		}

		public DictionarySerializer(Dictionary<TKey, TValue> source)
		{
			_Source = source;
		}

		[XmlIgnore]
		public Dictionary<TKey, TValue> Source
		{
			get
			{
				return _Source;
			}
		}


		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();

			if (wasEmpty)
				return;

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");
				reader.ReadStartElement("key");

				TKey key = (TKey)keySerializer.Deserialize(reader);
				reader.ReadEndElement();

				reader.ReadStartElement("value");
				TValue value = (TValue)valueSerializer.Deserialize(reader);
				reader.ReadEndElement();

				_Source.Add(key, value);

				reader.ReadEndElement();
				reader.MoveToContent();
			}

			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

			foreach (TKey key in _Source.Keys)
			{
				writer.WriteStartElement("item");
				
				writer.WriteStartElement("key");
				keySerializer.Serialize(writer, key);
				writer.WriteEndElement();

				writer.WriteStartElement("value");
				TValue value = _Source[key];
				XmlSerializer vs = new XmlSerializer(value.GetType());
				vs.Serialize(writer, value);
				//valueSerializer.Serialize(writer, value);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
		}

		#endregion
	}
}

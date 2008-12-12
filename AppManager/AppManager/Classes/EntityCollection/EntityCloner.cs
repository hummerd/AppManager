using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;


namespace AppManager.EntityCollection
{
	public static class EntityCloner
	{
		public static TEntity CloneEntity<TEntity>(TEntity src)
			where TEntity : class
		{
			XmlSerializer xs = new XmlSerializer(src.GetType());

			using (MemoryStream ms = new MemoryStream())
			{
				xs.Serialize(ms, src);
				return xs.Deserialize(ms) as TEntity;
			}
		}
	}
}

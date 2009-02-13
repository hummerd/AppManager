using System;
using System.Reflection;


namespace AppManager.EntityCollection
{
	public static class EntityMerger
	{
		public static void MergeEntity<TEntity>(TEntity src, TEntity dst)
			where TEntity : class
		{
			Type type = typeof(TEntity);
			PropertyInfo[] props = type.GetProperties();

			foreach (var prop in props)
			{
				object srcVal = prop.GetValue(src, null);
				object dstVal = prop.GetValue(dst, null);

				if (srcVal == dstVal)
					continue;

				if (srcVal == null && dstVal != null)
					prop.SetValue(dst, srcVal, null);
				else if (!srcVal.Equals(dstVal))
					prop.SetValue(dst, srcVal, null);
			}
		}
	}
}

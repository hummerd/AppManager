using System.Reflection;


namespace AppManager.Classes.Common
{
	public static class PropSetter
	{
		public static T GetValue<T>(object obj, string propName)
		{
			PropertyInfo pi = obj.GetType().GetProperty(propName);
			return (T)pi.GetValue(obj, null);
		}

		public static void SetValue<T>(object obj, string propName, T value)
		{
			PropertyInfo pi = obj.GetType().GetProperty(propName);
			pi.SetValue(obj, value, null);
		}
	}
}

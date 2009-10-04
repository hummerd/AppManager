using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace Updater
{
	public static class UIStrings
	{
		private static Dictionary<string, Dictionary<string, string>> _Strings = new Dictionary<string,Dictionary<string,string>>();


		static UIStrings()
		{
			var enStrings = new Dictionary<string, string>();
			enStrings.Add("UPDATER", "Updater");
			enStrings.Add("BAD_VERSION", "Downloaded version is corrupted.");
			enStrings.Add("RETRY", "Retry");
			enStrings.Add("CANCEL", "Cancel");
			enStrings.Add("WAIT_MSG", "Close following programs to proceed and press Retry");

			_Strings.Add("en", enStrings);


			var ruStrings = new Dictionary<string, string>();
			ruStrings.Add("UPDATER", "Обновление");
			ruStrings.Add("BAD_VERSION", "Загруженная версия повреждениа.");
			ruStrings.Add("RETRY", "Повтор");
			ruStrings.Add("CANCEL", "Отмена");
			ruStrings.Add("WAIT_MSG", "Для продолжения закройте указанные программы и нажмите Повтор");

			_Strings.Add("ru", ruStrings);			
		}

		public static string Str(string key)
		{
			var lang = CultureInfo.CurrentUICulture.Parent.Name;
			return _Strings[lang == "ru" ? lang : "en"][key];
		}
	}
}

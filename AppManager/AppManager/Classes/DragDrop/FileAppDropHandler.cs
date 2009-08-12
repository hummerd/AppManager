using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CommonLib.IO;
using DragDropLib;


namespace AppManager.DragDrop
{
	public class FileAppDropHandler : FileDropHandler
	{
		protected string _TempLink;


		public override DragDropEffects SupportDataFormat(FrameworkElement element, DragEventArgs dragData)
		{
			if ((dragData.KeyStates & DragDropKeyStates.AltKey) == DragDropKeyStates.AltKey)
			{
				var input = (element as ItemsControl).InputHitTest(dragData.GetPosition(element)) as FrameworkElement;
				if (input != null)
				{
					var appInfo = input.DataContext as AppInfo;
					if (appInfo != null)
						return DragDropEffects.Link;
				}
			}

			return base.SupportDataFormat(element, dragData);
		}

		public override void SetDragData(DragDropLib.DataObject dragData, object dragObject)
		{
			base.SetDragData(dragData, dragObject);
			
			var ai = dragObject as AppInfo;
			if (ai != null)
			{
				_TempLink = Path.Combine(Path.GetTempPath(), ai.AppName + ".lnk");
				LnkHelper.CreateLnk(_TempLink, ai.AppPath, ai.AppArgs);
				dragData.SetDataEx(DataFormats.FileDrop, new string[] { _TempLink });
			}
		}

		public override void DragEnded(DragDropEffects effects)
		{
			base.DragEnded(effects);

			if (!String.IsNullOrEmpty(_TempLink))
			{
				if (File.Exists(_TempLink))
					File.Delete(_TempLink);
			}
		}
	}
}

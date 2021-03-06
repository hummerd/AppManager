﻿using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AppManager.Entities;
using CommonLib.IO;
using DragDropLib;


namespace AppManager.DragDrop
{
	public class FileAppDropHandler : FileDropHandler
	{
		protected Thread _LnkWorker;
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
				InitCreateLnkThread();
				_LnkWorker.Start(ai);
				dragData.SetDataEx(DataFormats.FileDrop, new string[] { _TempLink });
			}
		}

		public override void DragEnded(DragDropEffects effects, FrameworkElement dragSource)
		{
			base.DragEnded(effects, dragSource);

			if (!String.IsNullOrEmpty(_TempLink))
			{
				if (File.Exists(_TempLink))
					File.Delete(_TempLink);
			}
		}


		protected void InitCreateLnkThread()
		{
			_LnkWorker = new Thread(CreateLnk);
			_LnkWorker.SetApartmentState(ApartmentState.STA);
			_LnkWorker.IsBackground = true;
		}


		private static void CreateLnk(object state)
		{
			try
			{
				AppInfo ai = state as AppInfo;
				string tempLink = Path.Combine(Path.GetTempPath(), ai.AppName + ".lnk");
				LnkHelper.CreateLnk(tempLink, ai.AppPath, ai.LoadImagePath, ai.AppArgs);
			}
			catch
			{ ; }
		}
	}
}

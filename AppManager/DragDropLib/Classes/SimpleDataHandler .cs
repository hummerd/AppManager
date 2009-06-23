using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using CommonLib;


namespace DragDropLib
{
	public class SimpleDragDataHandler : IDragHandler
	{
		public event EventHandler<DragContextEventArgs> ObjectDroped;


		protected string	_DataFormat;
		protected Type		_DataType;


		public SimpleDragDataHandler(string dataFormat, Type dataType)
		{
			_DataFormat = dataFormat;
			_DataType = dataType;
		}


		#region IDragHandler Members

		public DragDropEffects SupportDataFormat(FrameworkElement element, DragEventArgs dragData)
		{
			if (!dragData.Data.GetDataPresent(_DataFormat))
				return DragDropEffects.None;

			if ((dragData.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
				return DragDropEffects.Copy;
			
			return DragDropEffects.Move;
		}

		public bool HandleDragData(FrameworkElement element, DragEventArgs dragData)
		{
			if (!dragData.Data.GetDataPresent(_DataFormat))
				return false;

			object objAdd = DataObject.ReadFromStream(dragData.Data.GetData(_DataFormat) as MemoryStream);
			objAdd = XmlSerializeHelper.DeserializeItem(objAdd.ToString(), _DataType);

			if (ObjectDroped != null)
				ObjectDroped(element, new DragContextEventArgs() 
					{ DropObject = objAdd, EventArguments = dragData });

			return true;
		}

		public void SetDragData(DataObject dragData, object dragObject)
		{
			if (dragObject == null)
				return;

			if (dragObject.GetType() != _DataType)
				return;

			string serObj = XmlSerializeHelper.SerializeItem(dragObject);
			dragData.SetManagedData(_DataFormat, serObj);
		}

		#endregion
	}
}

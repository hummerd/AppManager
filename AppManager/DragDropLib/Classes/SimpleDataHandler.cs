using System;
using System.IO;
using System.Windows;
using CommonLib;


namespace DragDropLib
{
	public class SimpleDragDataHandler : IDragHandler
	{
		public event EventHandler<DragContextEventArgs> ObjectDroped;


		protected string	_DataFormat;
		protected Type		_DataType;
		protected IObjectSerializer _Serializer;

		public SimpleDragDataHandler(string dataFormat, Type dataType)
			: this(dataFormat, dataType, new XmlObjectSerializer())
		{ }

		public SimpleDragDataHandler(string dataFormat, Type dataType, IObjectSerializer serializer)
		{
			_DataFormat = dataFormat;
			_DataType = dataType;
			_Serializer = serializer;
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
			objAdd = _Serializer.Deserialize(objAdd, _DataType);

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

			string serObj = _Serializer.Serialize(dragObject).ToString();
			dragData.SetManagedData(_DataFormat, serObj);
		}

		public void DragEnded(DragDropEffects effects)
		{ ; }

		#endregion
	}

	public interface IObjectSerializer
	{
		object Serialize(object obj);
		object Deserialize(object data, Type objType);
	}

	public class  XmlObjectSerializer : IObjectSerializer
	{
		public object Serialize(object obj)
		{
			return XmlSerializeHelper.SerializeItem(obj);
		}

		public object Deserialize(object data, Type objType)
		{
			return XmlSerializeHelper.DeserializeItem(data.ToString(), objType);
		}
	}
}

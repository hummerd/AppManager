using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using ComTypes = System.Runtime.InteropServices.ComTypes;


namespace DragDropLib
{
    /// <summary>
    /// Implements the COM version of IDataObject including SetData.
    /// </summary>
    /// <remarks>
    /// <para>Use this object when using shell (or other unmanged) features
    /// that utilize the clipboard and/or drag and drop.</para>
    /// <para>The System.Windows.DataObject (.NET 3.0) and
    /// System.Windows.Forms.DataObject do not support SetData from their COM
    /// IDataObject interface implementation.</para>
    /// <para>To use this object with .NET drag and drop, create an instance
    /// of System.Windows.DataObject (.NET 3.0) or System.Window.Forms.DataObject
    /// passing an instance of DataObject as the only constructor parameter. For
    /// example:</para>
    /// <code>
    /// System.Windows.DataObject data = new System.Windows.DataObject(new DragDropLib.DataObject());
    /// </code>
    /// </remarks>
	[ComVisible(true)]
	public class DataObject : ComTypes.IDataObject, IDisposable
	{
		// Our internal storage is a simple list
		private IList<KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM>> storage;

		/// <summary>
		/// Creates an empty instance of DataObject.
		/// </summary>
		public DataObject()
		{
			storage = new List<KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM>>();
		}

		/// <summary>
		/// Releases unmanaged resources.
		/// </summary>
		~DataObject()
		{
			Dispose(false);
		}

		/// <summary>
		/// Clears the internal storage array.
		/// </summary>
		/// <remarks>
		/// ClearStorage is called by the IDisposable.Dispose method implementation
		/// to make sure all unmanaged references are released properly.
		/// </remarks>
		private void ClearStorage()
		{
			foreach (KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM> pair in storage)
			{
				ComTypes.STGMEDIUM medium = pair.Value;
				ReleaseStgMedium(ref medium);
			}
			storage.Clear();
		}

		/// <summary>
		/// Releases resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Releases resources.
		/// </summary>
		/// <param name="disposing">Indicates if the call was made by a managed caller, or the garbage collector.
		/// True indicates that someone called the Dispose method directly. False indicates that the garbage collector
		/// is finalizing the release of the object instance.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				// No managed objects to release
			}

			// Always release unmanaged objects
			ClearStorage();
		}

		#region COM IDataObject Members

		#region COM constants

		private const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);

		private const int DV_E_FORMATETC = unchecked((int)0x80040064);
		private const int DV_E_TYMED = unchecked((int)0x80040069);
		private const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);
		private const int DV_E_DVASPECT = unchecked((int)0x8004006B);

		#endregion // COM constants

		#region Unsupported functions

		public int DAdvise(ref ComTypes.FORMATETC pFormatetc, ComTypes.ADVF advf, ComTypes.IAdviseSink adviseSink, out int connection)
		{
			throw Marshal.GetExceptionForHR(OLE_E_ADVISENOTSUPPORTED);
		}

		public void DUnadvise(int connection)
		{
			throw Marshal.GetExceptionForHR(OLE_E_ADVISENOTSUPPORTED);
		}

		public int EnumDAdvise(out ComTypes.IEnumSTATDATA enumAdvise)
		{
			throw Marshal.GetExceptionForHR(OLE_E_ADVISENOTSUPPORTED);
		}

		public int GetCanonicalFormatEtc(ref ComTypes.FORMATETC formatIn, out ComTypes.FORMATETC formatOut)
		{
			formatOut = formatIn;
			return DV_E_FORMATETC;
		}

		public void GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium)
		{
			throw new NotSupportedException();
		}

		#endregion // Unsupported functions

		/// <summary>
		/// Gets an enumerator for the formats contained in this DataObject.
		/// </summary>
		/// <param name="direction">The direction of the data.</param>
		/// <returns>An instance of the IEnumFORMATETC interface.</returns>
		public ComTypes.IEnumFORMATETC EnumFormatEtc(ComTypes.DATADIR direction)
		{
			// We only support GET
			if (ComTypes.DATADIR.DATADIR_GET == direction)
				return new EnumFORMATETC(storage);

			throw new NotImplementedException("OLE_S_USEREG");
		}

		/// <summary>
		/// Gets the specified data.
		/// </summary>
		/// <param name="format">The requested data format.</param>
		/// <param name="medium">When the function returns, contains the requested data.</param>
		public void GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium)
		{
			// Locate the data
			foreach (KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM> pair in storage)
			{
				if ((pair.Key.tymed & format.tymed) > 0
					 && pair.Key.dwAspect == format.dwAspect
					 && pair.Key.cfFormat == format.cfFormat)
				{
					// Found it. Return a copy of the data.
					ComTypes.STGMEDIUM source = pair.Value;
					medium = CopyMedium(ref source);
					return;
				}
			}

			// Didn't find it. Return an empty data medium.
			medium = new ComTypes.STGMEDIUM();
		}

		/// <summary>
		/// Determines if data of the requested format is present.
		/// </summary>
		/// <param name="format">The request data format.</param>
		/// <returns>Returns the status of the request. If the data is present, S_OK is returned.
		/// If the data is not present, an error code with the best guess as to the reason is returned.</returns>
		public int QueryGetData(ref ComTypes.FORMATETC format)
		{
			// We only support CONTENT aspect
			if ((ComTypes.DVASPECT.DVASPECT_CONTENT & format.dwAspect) == 0)
				return DV_E_DVASPECT;

			int ret = DV_E_TYMED;

			// Try to locate the data
			// TODO: The ret, if not S_OK, is only relevant to the last item
			foreach (KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM> pair in storage)
			{
				if ((pair.Key.tymed & format.tymed) > 0)
				{
					if (pair.Key.cfFormat == format.cfFormat)
					{
						// Found it, return S_OK;
						return 0;
					}
					else
					{
						// Found the medium type, but wrong format
						ret = DV_E_CLIPFORMAT;
					}
				}
				else
				{
					// Mismatch on medium type
					ret = DV_E_TYMED;
				}
			}

			return ret;
		}

		/// <summary>
		/// Sets data in the specified format into storage.
		/// </summary>
		/// <param name="formatIn">The format of the data.</param>
		/// <param name="medium">The data.</param>
		/// <param name="release">If true, ownership of the medium's memory will be transferred
		/// to this object. If false, a copy of the medium will be created and maintained, and
		/// the caller is responsible for the memory of the medium it provided.</param>
		public void SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release)
		{
			// If the format exists in our storage, remove it prior to resetting it
			foreach (KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM> pair in storage)
			{
				if ((pair.Key.tymed & formatIn.tymed) > 0
					 && pair.Key.dwAspect == formatIn.dwAspect
					 && pair.Key.cfFormat == formatIn.cfFormat)
				{
					storage.Remove(pair);
					break;
				}
			}

			// If release is true, we'll take ownership of the medium.
			// If not, we'll make a copy of it.
			ComTypes.STGMEDIUM sm = medium;
			if (!release)
				sm = CopyMedium(ref medium);

			// Add it to the internal storage
			KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM> addPair = new KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM>(formatIn, sm);
			storage.Add(addPair);
		}

		/// <summary>
		/// Creates a copy of the STGMEDIUM structure.
		/// </summary>
		/// <param name="medium">The data to copy.</param>
		/// <returns>The copied data.</returns>
		private ComTypes.STGMEDIUM CopyMedium(ref ComTypes.STGMEDIUM medium)
		{
			ComTypes.STGMEDIUM sm = new ComTypes.STGMEDIUM();
			int hr = CopyStgMedium(ref medium, ref sm);
			if (hr != 0)
				throw Marshal.GetExceptionForHR(hr);

			return sm;

		}

		#endregion

		/// <summary>
		/// Helps enumerate the formats available in our DataObject class.
		/// </summary>
		[ComVisible(true)]
		private class EnumFORMATETC : ComTypes.IEnumFORMATETC
		{
			// Keep an array of the formats for enumeration
			private ComTypes.FORMATETC[] formats;
			// The index of the next item
			private int currentIndex = 0;

			/// <summary>
			/// Creates an instance from a list of key value pairs.
			/// </summary>
			/// <param name="storage">List of FORMATETC/STGMEDIUM key value pairs</param>
			internal EnumFORMATETC(IList<KeyValuePair<ComTypes.FORMATETC, ComTypes.STGMEDIUM>> storage)
			{
				// Get the formats from the list
				formats = new ComTypes.FORMATETC[storage.Count];
				for (int i = 0; i < formats.Length; i++)
					formats[i] = storage[i].Key;
			}

			/// <summary>
			/// Creates an instance from an array of FORMATETC's.
			/// </summary>
			/// <param name="formats">Array of formats to enumerate.</param>
			private EnumFORMATETC(ComTypes.FORMATETC[] formats)
			{
				// Get the formats as a copy of the array
				this.formats = new ComTypes.FORMATETC[formats.Length];
				formats.CopyTo(this.formats, 0);
			}

			#region IEnumFORMATETC Members

			/// <summary>
			/// Creates a clone of this enumerator.
			/// </summary>
			/// <param name="newEnum">When this function returns, contains a new instance of IEnumFORMATETC.</param>
			public void Clone(out ComTypes.IEnumFORMATETC newEnum)
			{
				EnumFORMATETC ret = new EnumFORMATETC(formats);
				ret.currentIndex = currentIndex;
				newEnum = ret;
			}

			/// <summary>
			/// Retrieves the next elements from the enumeration.
			/// </summary>
			/// <param name="celt">The number of elements to retrieve.</param>
			/// <param name="rgelt">An array to receive the formats requested.</param>
			/// <param name="pceltFetched">An array to receive the number of element fetched.</param>
			/// <returns>If the fetched number of formats is the same as the requested number, S_OK is returned.
			/// There are several reasons S_FALSE may be returned: (1) The requested number of elements is less than
			/// or equal to zero. (2) The rgelt parameter equals null. (3) There are no more elements to enumerate.
			/// (4) The requested number of elements is greater than one and pceltFetched equals null or does not
			/// have at least one element in it. (5) The number of fetched elements is less than the number of
			/// requested elements.</returns>
			public int Next(int celt, ComTypes.FORMATETC[] rgelt, int[] pceltFetched)
			{
				// Start with zero fetched, in case we return early
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = 0;

				// This will count down as we fetch elements
				int cReturn = celt;

				// Short circuit if they didn't request any elements, or didn't
				// provide room in the return array, or there are not more elements
				// to enumerate.
				if (celt <= 0 || rgelt == null || currentIndex >= formats.Length)
					return 1; // S_FALSE

				// If the number of requested elements is not one, then we must
				// be able to tell the caller how many elements were fetched.
				if ((pceltFetched == null || pceltFetched.Length < 1) && celt != 1)
					return 1; // S_FALSE

				// If the number of elements in the return array is too small, we
				// throw. This is not a likely scenario, hence the exception.
				if (rgelt.Length < celt)
					throw new ArgumentException("The number of elements in the return array is less than the number of elements requested");

				// Fetch the elements.
				for (int i = 0; currentIndex < formats.Length && cReturn > 0; i++, cReturn--, currentIndex++)
					rgelt[i] = formats[currentIndex];

				// Return the number of elements fetched
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = celt - cReturn;

				// cReturn has the number of elements requested but not fetched.
				// It will be greater than zero, if multiple elements were requested
				// but we hit the end of the enumeration.
				return (cReturn == 0) ? 0 : 1; // S_OK : S_FALSE
			}

			/// <summary>
			/// Resets the state of enumeration.
			/// </summary>
			/// <returns>S_OK</returns>
			public int Reset()
			{
				currentIndex = 0;
				return 0; // S_OK
			}

			/// <summary>
			/// Skips the number of elements requested.
			/// </summary>
			/// <param name="celt">The number of elements to skip.</param>
			/// <returns>If there are not enough remaining elements to skip, returns S_FALSE. Otherwise, S_OK is returned.</returns>
			public int Skip(int celt)
			{
				if (currentIndex + celt > formats.Length)
					return 1; // S_FALSE

				currentIndex += celt;
				return 0; // S_OK
			}

			#endregion
		}



		#region DLL imports

		[DllImport("user32.dll")]
		private static extern uint RegisterClipboardFormat(string lpszFormatName);

		[DllImport("urlmon.dll")]
		private static extern int CopyStgMedium(ref ComTypes.STGMEDIUM pcstgmedSrc, ref ComTypes.STGMEDIUM pstgmedDest);

		[DllImport("ole32.dll")]
		private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM pmedium);

		[DllImport("ole32.dll")]
		private static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out ComTypes.IStream ppstm);

		#endregion // DLL imports

		#region Native constants

		// CFSTR_DROPDESCRIPTION
		private const string DropDescriptionFormat = "DropDescription";

		#endregion // Native constants

		///// <summary>
		///// Sets the drop description for the drag image manager.
		///// </summary>
		///// <param name="dataObject">The DataObject to set.</param>
		///// <param name="dropDescription">The drop description.</param>
		//public static void SetDropDescription(this IDataObject dataObject, DropDescription dropDescription)
		//{
		//   ComTypes.FORMATETC formatETC;
		//   FillFormatETC(DropDescriptionFormat, ComTypes.TYMED.TYMED_HGLOBAL, out formatETC);

		//   // We need to set the drop description as an HGLOBAL.
		//   // Allocate space ...
		//   IntPtr pDD = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DropDescription)));
		//   try
		//   {
		//      // ... and marshal the data
		//      Marshal.StructureToPtr(dropDescription, pDD, false);

		//      // The medium wraps the HGLOBAL
		//      System.Runtime.InteropServices.ComTypes.STGMEDIUM medium;
		//      medium.pUnkForRelease = null;
		//      medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
		//      medium.unionmember = pDD;

		//      // Set the data
		//      ComTypes.IDataObject dataObjectCOM = (ComTypes.IDataObject)dataObject;
		//      dataObjectCOM.SetData(ref formatETC, ref medium, true);
		//   }
		//   catch
		//   {
		//      // If we failed, we need to free the HGLOBAL memory
		//      Marshal.FreeHGlobal(pDD);
		//      throw;
		//   }
		//}

		///// <summary>
		///// Gets the DropDescription format data.
		///// </summary>
		///// <param name="dataObject">The DataObject.</param>
		///// <returns>The DropDescription, if set.</returns>
		//public static object GetDropDescription(this IDataObject dataObject)
		//{
		//   ComTypes.FORMATETC formatETC;
		//   FillFormatETC(DropDescriptionFormat, ComTypes.TYMED.TYMED_HGLOBAL, out formatETC);

		//   if (0 == dataObject.QueryGetData(ref formatETC))
		//   {
		//      ComTypes.STGMEDIUM medium;
		//      dataObject.GetData(ref formatETC, out medium);
		//      try
		//      {
		//         return (DropDescription)Marshal.PtrToStructure(medium.unionmember, typeof(DropDescription));
		//      }
		//      finally
		//      {
		//         ReleaseStgMedium(ref medium);
		//      }
		//   }

		//   return null;
		//}

		// Combination of all non-null TYMEDs
		private const ComTypes.TYMED TYMED_ANY =
			 ComTypes.TYMED.TYMED_ENHMF
			 | ComTypes.TYMED.TYMED_FILE
			 | ComTypes.TYMED.TYMED_GDI
			 | ComTypes.TYMED.TYMED_HGLOBAL
			 | ComTypes.TYMED.TYMED_ISTORAGE
			 | ComTypes.TYMED.TYMED_ISTREAM
			 | ComTypes.TYMED.TYMED_MFPICT;

		///// <summary>
		///// Sets up an advisory connection to the data object.
		///// </summary>
		///// <param name="dataObject">The data object on which to set the advisory connection.</param>
		///// <param name="sink">The advisory sink.</param>
		///// <param name="format">The format on which to callback on.</param>
		///// <param name="advf">Advisory flags. Can be 0.</param>
		///// <returns>The ID of the newly created advisory connection.</returns>
		//public static int Advise(ComTypes.IAdviseSink sink, string format, ComTypes.ADVF advf)
		//{
		//   // Internally, we'll listen for any TYMED
		//   ComTypes.FORMATETC formatETC;
		//   FillFormatETC(format, TYMED_ANY, out formatETC);

		//   int connection;
		//   int hr = DAdvise(ref formatETC, advf, sink, out connection);
		//   if (hr != 0)
		//      Marshal.ThrowExceptionForHR(hr);
		//   return connection;
		//}

		/// <summary>
		/// Fills a FORMATETC structure.
		/// </summary>
		/// <param name="format">The format name.</param>
		/// <param name="tymed">The accepted TYMED.</param>
		/// <param name="formatETC">The structure to fill.</param>
		private static void FillFormatETC(string format, ComTypes.TYMED tymed, out ComTypes.FORMATETC formatETC)
		{
			formatETC.cfFormat = (short)RegisterClipboardFormat(format);
			formatETC.dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT;
			formatETC.lindex = -1;
			formatETC.ptd = IntPtr.Zero;
			formatETC.tymed = tymed;
		}

		// Identifies data that we need to do custom marshaling on
		private static readonly Guid ManagedDataStamp = new Guid("D98D9FD6-FA46-4716-A769-F3451DFBE4B4");

		/// <summary>
		/// Sets managed data to a clipboard DataObject.
		/// </summary>
		/// <param name="dataObject">The DataObject to set the data on.</param>
		/// <param name="format">The clipboard format.</param>
		/// <param name="data">The data object.</param>
		/// <remarks>
		/// Because the underlying data store is not storing managed objects, but
		/// unmanaged ones, this function provides intelligent conversion, allowing
		/// you to set unmanaged data into the COM implemented IDataObject.</remarks>
		public void SetManagedData(string format, object data)
		{
			// Initialize the format structure
			ComTypes.FORMATETC formatETC;
			FillFormatETC(format, ComTypes.TYMED.TYMED_HGLOBAL, out formatETC);

			// Serialize/marshal our data into an unmanaged medium
			ComTypes.STGMEDIUM medium;
			GetMediumFromObject(data, out medium);
			try
			{
				// Set the data on our data object
				SetData(ref formatETC, ref medium, true);
			}
			catch
			{
				// On exceptions, release the medium
				ReleaseStgMedium(ref medium);
				throw;
			}
		}

		/// <summary>
		/// Gets managed data from a clipboard DataObject.
		/// </summary>
		/// <param name="dataObject">The DataObject to obtain the data from.</param>
		/// <param name="format">The format for which to get the data in.</param>
		/// <returns>The data object instance.</returns>
		public object GetManagedData(string format)
		{
			ComTypes.FORMATETC formatETC;
			FillFormatETC(format, ComTypes.TYMED.TYMED_HGLOBAL, out formatETC);

			// Get the data as a stream
			ComTypes.STGMEDIUM medium;
			GetData(ref formatETC, out medium);

			ComTypes.IStream nativeStream;
			try
			{
				int hr = CreateStreamOnHGlobal(medium.unionmember, true, out nativeStream);
				if (hr != 0)
				{
					return null;
				}
			}
			finally
			{
				ReleaseStgMedium(ref medium);
			}


			// Convert the native stream to a managed stream            
			ComTypes.STATSTG statstg;
			nativeStream.Stat(out statstg, 0);
			if (statstg.cbSize > int.MaxValue)
				throw new NotSupportedException();
			byte[] buf = new byte[statstg.cbSize];
			nativeStream.Read(buf, (int)statstg.cbSize, IntPtr.Zero);
			MemoryStream dataStream = new MemoryStream(buf);
			return ReadFromStream(dataStream);
		}

		public static object ReadFromStream(Stream dataStream)
		{
			int sizeOfGuid = Marshal.SizeOf(typeof(Guid));
			byte[] guidBytes = new byte[sizeOfGuid];
			if (dataStream.Length >= sizeOfGuid)
			{
				if (sizeOfGuid == dataStream.Read(guidBytes, 0, sizeOfGuid))
				{
					Guid guid = new Guid(guidBytes);
					if (ManagedDataStamp.Equals(guid))
					{
						// Stamp matched, so deserialize
						BinaryFormatter formatter = new BinaryFormatter();
						Type dataType = (Type)formatter.Deserialize(dataStream);
						object data2 = formatter.Deserialize(dataStream);
						if (data2.GetType() == dataType)
							return data2;
						else if (data2 is string)
							return ConvertDataFromString((string)data2, dataType);
						else
							return null;
					}
				}
			}

			// Stamp didn't match... attempt to reset the seek pointer
			if (dataStream.CanSeek)
				dataStream.Position = 0;
			return null;
		}

		#region Helper methods

		/// <summary>
		/// Serializes managed data to an HGLOBAL.
		/// </summary>
		/// <param name="data">The managed data object.</param>
		/// <returns>An STGMEDIUM pointing to the allocated HGLOBAL.</returns>
		private static void GetMediumFromObject(object data, out ComTypes.STGMEDIUM medium)
		{
			// We'll serialize to a managed stream temporarily
			MemoryStream stream = new MemoryStream();

			// Write an indentifying stamp, so we can recognize this as custom
			// marshaled data.
			stream.Write(ManagedDataStamp.ToByteArray(), 0, Marshal.SizeOf(typeof(Guid)));

			// Now serialize the data. Note, if the data is not directly serializable,
			// we'll try type conversion. Also, we serialize the type. That way,
			// during deserialization, we know which type to convert back to, if
			// appropriate.
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, data.GetType());
			formatter.Serialize(stream, GetAsSerializable(data));

			// Now copy to an HGLOBAL
			byte[] bytes = stream.GetBuffer();
			IntPtr p = Marshal.AllocHGlobal(bytes.Length);
			try
			{
				Marshal.Copy(bytes, 0, p, bytes.Length);
			}
			catch
			{
				// Make sure to free the memory on exceptions
				Marshal.FreeHGlobal(p);
				throw;
			}

			// Now allocate an STGMEDIUM to wrap the HGLOBAL
			medium.unionmember = p;
			medium.tymed = ComTypes.TYMED.TYMED_HGLOBAL;
			medium.pUnkForRelease = null;
		}

		/// <summary>
		/// Gets a serializable object representing the data.
		/// </summary>
		/// <param name="obj">The data.</param>
		/// <returns>If the data is serializable, then it is returned. Otherwise,
		/// type conversion is attempted. If successful, a string value will be
		/// returned.</returns>
		private static object GetAsSerializable(object obj)
		{
			// If the data is directly serializable, run with it
			if (obj.GetType().IsSerializable)
				return obj;

			// Attempt type conversion to a string, but only if we know it can be converted back
			TypeConverter conv = GetTypeConverterForType(obj.GetType());
			if (conv != null && conv.CanConvertTo(typeof(string)) && conv.CanConvertFrom(typeof(string)))
				return conv.ConvertToInvariantString(obj);

			throw new NotSupportedException("Cannot serialize the object");
		}

		/// <summary>
		/// Converts data from a string to the specified format.
		/// </summary>
		/// <param name="data">The data to convert.</param>
		/// <param name="dataType">The target data type.</param>
		/// <returns>Returns the converted data instance.</returns>
		private static object ConvertDataFromString(string data, Type dataType)
		{
			TypeConverter conv = GetTypeConverterForType(dataType);
			if (conv != null && conv.CanConvertFrom(typeof(string)))
				return conv.ConvertFromInvariantString(data);

			throw new NotSupportedException("Cannot convert data");
		}

		/// <summary>
		/// Gets a TypeConverter instance for the specified type.
		/// </summary>
		/// <param name="dataType">The type.</param>
		/// <returns>An instance of a TypeConverter for the type, if one exists.</returns>
		private static TypeConverter GetTypeConverterForType(Type dataType)
		{
			TypeConverterAttribute[] typeConverterAttrs = (TypeConverterAttribute[])dataType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
			if (typeConverterAttrs.Length > 0)
			{
				Type convType = Type.GetType(typeConverterAttrs[0].ConverterTypeName);
				return (TypeConverter)Activator.CreateInstance(convType);
			}

			return null;
		}

		#endregion // Helper methods
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;


namespace CommonLib.IO
{
	public static class LnkHelper
	{
		private static WshShellClass _Shell;


		public static IWshShortcut OpenLnk(string path)
		{
			if (_Shell == null)
				_Shell = new WshShellClass();

			return _Shell.CreateShortcut(path) as IWshShortcut;
		}

		public static void CreateLnk(string path, string lnkPath, string args)
		{
			if (_Shell == null)
				_Shell = new WshShellClass();

			var lnk = _Shell.CreateShortcut(path) as IWshShortcut;
			lnk.TargetPath = lnkPath;
			lnk.IconLocation = lnkPath;
			lnk.Arguments = args;
			lnk.Save();
		}
	}


	[ComImport, Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"), TypeLibType((short)2), ClassInterface((short)0)]
	public class WshShellClass : IWshShell3
	{
		// Methods
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc2)]
		public virtual extern bool AppActivate([In, MarshalAs(UnmanagedType.Struct)] ref object App, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Wait);

		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)]
		public virtual extern object CreateShortcut([In, MarshalAs(UnmanagedType.BStr)] string PathLink);

		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc4)]
		public virtual extern IWshExec Exec([In, MarshalAs(UnmanagedType.BStr)] string Command);

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)]
		public virtual extern string ExpandEnvironmentStrings([In, MarshalAs(UnmanagedType.BStr)] string Src);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbb8)]
		public virtual extern bool LogEvent([In, MarshalAs(UnmanagedType.Struct)] ref object Type, [In, MarshalAs(UnmanagedType.BStr)] string Message, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string Target);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)]
		public virtual extern int Popup([In, MarshalAs(UnmanagedType.BStr)] string Text, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object SecondsToWait, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Title, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d2)]
		public virtual extern void RegDelete([In, MarshalAs(UnmanagedType.BStr)] string Name);

		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d0)]
		public virtual extern object RegRead([In, MarshalAs(UnmanagedType.BStr)] string Name);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d1)]
		public virtual extern void RegWrite([In, MarshalAs(UnmanagedType.BStr)] string Name, [In, MarshalAs(UnmanagedType.Struct)] ref object Value, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)]
		public virtual extern int Run([In, MarshalAs(UnmanagedType.BStr)] string Command, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WindowStyle, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WaitOnReturn);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc3)]
		public virtual extern void SendKeys([In, MarshalAs(UnmanagedType.BStr)] string Keys, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Wait);

		// Properties
		[DispId(0xbc5)]
		public virtual extern string CurrentDirectory { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc5)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc5)] set; }

		[DispId(200)]
		public virtual extern IWshEnvironment this[object Type] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(200)] get; }

		[DispId(100)]
		public virtual extern IWshCollection SpecialFolders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)] get; }
	}

	[ComImport, ClassInterface((short)0), DefaultMember("FullName"), Guid("A548B8E4-51D5-4661-8824-DAA1D893DFB2"), TypeLibType((short)2)]
	public class WshShortcutClass : IWshShortcut
	{
		// Methods
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0x7d0)]
		public virtual extern void Load([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d1)]
		public virtual extern void Save();

		// Properties
		[DispId(0x3e8)]
		public virtual extern string Arguments { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)] set; }
		[DispId(0x3e9)]
		public virtual extern string Description { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)] set; }
		[DispId(0)]
		public virtual extern string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
		[DispId(0x3ea)]
		public virtual extern string Hotkey { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)] set; }
		[DispId(0x3eb)]
		public virtual extern string IconLocation { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3eb)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3eb)] set; }
		//[DispId(0x3e8)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.Arguments { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)] set; }
		//[DispId(0x3e9)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.Description { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)] set; }
		//[DispId(0)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
		//[DispId(0x3ea)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.Hotkey { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)] set; }
		//[DispId(0x3eb)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.IconLocation { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3eb)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3eb)] set; }
		//[DispId(0x3ec)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.RelativePath { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ec)] set; }
		//[DispId(0x3ed)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.TargetPath { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ed)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ed)] set; }
		//[DispId(0x3ee)]
		//public virtual int IWshRuntimeLibrary.IWshShortcut.WindowStyle { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)] set; }
		//[DispId(0x3ef)]
		//public virtual string IWshRuntimeLibrary.IWshShortcut.WorkingDirectory { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ef)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ef)] set; }
		[DispId(0x3ec)]
		public virtual extern string RelativePath { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ec)] set; }
		[DispId(0x3ed)]
		public virtual extern string TargetPath { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ed)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ed)] set; }
		[DispId(0x3ee)]
		public virtual extern int WindowStyle { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)] set; }
		[DispId(0x3ef)]
		public virtual extern string WorkingDirectory { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ef)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ef)] set; }
	}

	[ComImport, Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B"), DefaultMember("FullName"), TypeLibType((short)0x1040)]
	public interface IWshShortcut
	{
		[DispId(0)]
		string FullName { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }

		[DispId(0x3e8)]
		string Arguments { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)] set; }

		[DispId(0x3e9)]
		string Description { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)] set; }

		[DispId(0x3ea)]
		string Hotkey { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)] set; }

		[DispId(0x3eb)]
		string IconLocation { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3eb)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3eb)] set; }

		[DispId(0x3ec)]
		string RelativePath { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ec)] set; }

		[DispId(0x3ed)]
		string TargetPath { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ed)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ed)] set; }

		[DispId(0x3ee)]
		int WindowStyle { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)] set; }

		[DispId(0x3ef)]
		string WorkingDirectory { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ef)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ef)] set; }

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(0x7d0)]
		void Load([In, MarshalAs(UnmanagedType.BStr)] string PathLink);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d1)]
		void Save();
	}

	[ComImport, Guid("41904400-BE18-11D3-A28B-00104BD35090"), TypeLibType((short)0x1040)]
	public interface IWshShell3 : IWshShell2
	{
		[DispId(100)]
		IWshCollection SpecialFolders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)] get; }
		[DispId(200)]
		IWshEnvironment this[object Type] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(200)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)]
		int Run([In, MarshalAs(UnmanagedType.BStr)] string Command, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WindowStyle, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WaitOnReturn);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)]
		int Popup([In, MarshalAs(UnmanagedType.BStr)] string Text, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object SecondsToWait, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Title, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)]
		object CreateShortcut([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)]
		string ExpandEnvironmentStrings([In, MarshalAs(UnmanagedType.BStr)] string Src);
		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d0)]
		object RegRead([In, MarshalAs(UnmanagedType.BStr)] string Name);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d1)]
		void RegWrite([In, MarshalAs(UnmanagedType.BStr)] string Name, [In, MarshalAs(UnmanagedType.Struct)] ref object Value, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d2)]
		void RegDelete([In, MarshalAs(UnmanagedType.BStr)] string Name);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbb8)]
		bool LogEvent([In, MarshalAs(UnmanagedType.Struct)] ref object Type, [In, MarshalAs(UnmanagedType.BStr)] string Message, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string Target);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc2)]
		bool AppActivate([In, MarshalAs(UnmanagedType.Struct)] ref object App, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Wait);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc3)]
		void SendKeys([In, MarshalAs(UnmanagedType.BStr)] string Keys, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Wait);
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc4)]
		IWshExec Exec([In, MarshalAs(UnmanagedType.BStr)] string Command);
		[DispId(0xbc5)]
		string CurrentDirectory { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc5)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc5)] set; }
	}

	[ComImport, TypeLibType((short)0x1050), Guid("24BE5A30-EDFE-11D2-B933-00104B365C9F")]
	public interface IWshShell2 : IWshShell
	{
		[DispId(100)]
		IWshCollection SpecialFolders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)] get; }
		[DispId(200)]
		IWshEnvironment this[object Type] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(200)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)]
		int Run([In, MarshalAs(UnmanagedType.BStr)] string Command, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WindowStyle, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WaitOnReturn);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)]
		int Popup([In, MarshalAs(UnmanagedType.BStr)] string Text, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object SecondsToWait, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Title, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)]
		object CreateShortcut([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)]
		string ExpandEnvironmentStrings([In, MarshalAs(UnmanagedType.BStr)] string Src);
		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d0)]
		object RegRead([In, MarshalAs(UnmanagedType.BStr)] string Name);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d1)]
		void RegWrite([In, MarshalAs(UnmanagedType.BStr)] string Name, [In, MarshalAs(UnmanagedType.Struct)] ref object Value, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d2)]
		void RegDelete([In, MarshalAs(UnmanagedType.BStr)] string Name);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbb8)]
		bool LogEvent([In, MarshalAs(UnmanagedType.Struct)] ref object Type, [In, MarshalAs(UnmanagedType.BStr)] string Message, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string Target);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc2)]
		bool AppActivate([In, MarshalAs(UnmanagedType.Struct)] ref object App, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Wait);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0xbc3)]
		void SendKeys([In, MarshalAs(UnmanagedType.BStr)] string Keys, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Wait);
	}

	[ComImport, TypeLibType((short)0x1050), Guid("F935DC21-1CF0-11D0-ADB9-00C04FD58A0B")]
	public interface IWshShell
	{
		[DispId(100)]
		IWshCollection SpecialFolders { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)] get; }
		[DispId(200)]
		IWshEnvironment this[object Type] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(200)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e8)]
		int Run([In, MarshalAs(UnmanagedType.BStr)] string Command, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WindowStyle, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object WaitOnReturn);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)]
		int Popup([In, MarshalAs(UnmanagedType.BStr)] string Text, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object SecondsToWait, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Title, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);
		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ea)]
		object CreateShortcut([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3ee)]
		string ExpandEnvironmentStrings([In, MarshalAs(UnmanagedType.BStr)] string Src);
		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d0)]
		object RegRead([In, MarshalAs(UnmanagedType.BStr)] string Name);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d1)]
		void RegWrite([In, MarshalAs(UnmanagedType.BStr)] string Name, [In, MarshalAs(UnmanagedType.Struct)] ref object Value, [In, Optional, MarshalAs(UnmanagedType.Struct)] ref object Type);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x7d2)]
		void RegDelete([In, MarshalAs(UnmanagedType.BStr)] string Name);
	}

	[ComImport, Guid("F935DC27-1CF0-11D0-ADB9-00C04FD58A0B"), DefaultMember("Item"), TypeLibType((short)0x1040)]
	public interface IWshCollection : IEnumerable
	{
		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)]
		object Item([In, MarshalAs(UnmanagedType.Struct)] ref object Index);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
		int Count();
		[DispId(2)]
		int length { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
		// [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType="", MarshalTypeRef=typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie="")]
		//[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-4)]
		//IEnumerator GetEnumerator();
	}

	[ComImport, Guid("F935DC29-1CF0-11D0-ADB9-00C04FD58A0B"), TypeLibType((short)0x1040)]
	public interface IWshEnvironment : IEnumerable
	{
		[DispId(0)]
		string this[string Name] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] set; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
		int Count();
		[DispId(2)]
		int length { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
		//[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType="", MarshalTypeRef=typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie="")]
		//[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-4)]
		//IEnumerator GetEnumerator();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x3e9)]
		void Remove([In, MarshalAs(UnmanagedType.BStr)] string Name);
	}

	[ComImport, TypeLibType((short)0x1040), Guid("08FED190-BE19-11D3-A28B-00104BD35090")]
	public interface IWshExec
	{
		[ComAliasName("IWshRuntimeLibrary.WshExecStatus"), DispId(1)]
		WshExecStatus Status { [return: ComAliasName("IWshRuntimeLibrary.WshExecStatus")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
		[DispId(3)]
		ITextStream StdIn { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; }
		[DispId(4)]
		ITextStream StdOut { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; }
		[DispId(5)]
		ITextStream StdErr { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; }
		[DispId(6)]
		int ProcessID { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; }
		[DispId(7)]
		int ExitCode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; }
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)]
		void Terminate();
	}

	[ComImport, TypeLibType((short)0x10d0), Guid("53BAD8C1-E718-11CF-893D-00A0C9054228")]
	public interface ITextStream
	{
		[DispId(0x2710)]
		int Line { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2710)] get; }
		[DispId(-529)]
		int Column { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-529)] get; }
		[DispId(0x2712)]
		bool AtEndOfStream { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2712)] get; }
		[DispId(0x2713)]
		bool AtEndOfLine { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2713)] get; }
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2714)]
		string Read([In] int Characters);
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2715)]
		string ReadLine();
		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2716)]
		string ReadAll();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2717)]
		void Write([In, MarshalAs(UnmanagedType.BStr)] string Text);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2718)]
		void WriteLine([In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string Text);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x2719)]
		void WriteBlankLines([In] int Lines);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x271a)]
		void Skip([In] int Characters);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x271b)]
		void SkipLine();
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x271c)]
		void Close();
	}

	public enum WshExecStatus
	{
		WshRunning,
		WshFinished,
		WshFailed
	}
}

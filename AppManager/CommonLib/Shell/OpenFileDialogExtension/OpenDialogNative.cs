using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.PInvoke;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;


namespace CommonLib.Shell.OpenFileDialogExtension
{
	public class OpenDialogNative : NativeWindow, IDisposable
	{
		#region Constants Declaration
		//private const SetWindowPosFlags UFLAGSSIZE =
		//    SetWindowPosFlags.SWP_NOACTIVATE |
		//    SetWindowPosFlags.SWP_NOOWNERZORDER |
		//    SetWindowPosFlags.SWP_NOMOVE;
		//private const SetWindowPosFlags UFLAGSSIZEEX =
		//    SetWindowPosFlags.SWP_NOACTIVATE |
		//    SetWindowPosFlags.SWP_NOOWNERZORDER |
		//    SetWindowPosFlags.SWP_NOMOVE |
		//    SetWindowPosFlags.SWP_ASYNCWINDOWPOS |
		//    SetWindowPosFlags.SWP_DEFERERASE;
		//private const SetWindowPosFlags UFLAGSMOVE =
		//    SetWindowPosFlags.SWP_NOACTIVATE |
		//    SetWindowPosFlags.SWP_NOOWNERZORDER |
		//    SetWindowPosFlags.SWP_NOSIZE;
		//private const SetWindowPosFlags UFLAGSHIDE =
		//    SetWindowPosFlags.SWP_NOACTIVATE |
		//    SetWindowPosFlags.SWP_NOOWNERZORDER |
		//    SetWindowPosFlags.SWP_NOMOVE |
		//    SetWindowPosFlags.SWP_NOSIZE |
		//    SetWindowPosFlags.SWP_HIDEWINDOW;
		//private const SetWindowPosFlags UFLAGSZORDER =
		//    SetWindowPosFlags.SWP_NOACTIVATE |
		//    SetWindowPosFlags.SWP_NOMOVE |
		//    SetWindowPosFlags.SWP_NOSIZE;
		#endregion

		#region Variables Declaration
		private Size mOriginalSize;
		private IntPtr mOpenDialogHandle;
		private IntPtr mListViewPtr;
		private User32.WINDOWINFO mListViewInfo;
		private BaseDialogNative mBaseDialogNative;
		private IntPtr mComboFolders;
		private User32.WINDOWINFO mComboFoldersInfo;
		private IntPtr mGroupButtons;
		private User32.WINDOWINFO mGroupButtonsInfo;
		private IntPtr mComboFileName;
		private User32.WINDOWINFO mComboFileNameInfo;
		private IntPtr mComboExtensions;
		private User32.WINDOWINFO mComboExtensionsInfo;
		private IntPtr mOpenButton;
		private User32.WINDOWINFO mOpenButtonInfo;
		private IntPtr mCancelButton;
		private User32.WINDOWINFO mCancelButtonInfo;
		private IntPtr mHelpButton;
		private User32.WINDOWINFO mHelpButtonInfo;
		private OpenFileDialogEx mSourceControl;
		private IntPtr mToolBarFolders;
		private User32.WINDOWINFO mToolBarFoldersInfo;
		private IntPtr mLabelFileName;
		private User32.WINDOWINFO mLabelFileNameInfo;
		private IntPtr mLabelFileType;
		private User32.WINDOWINFO mLabelFileTypeInfo;
		private IntPtr mChkReadOnly;
		private User32.WINDOWINFO mChkReadOnlyInfo;
		private bool mIsClosing = false;
		private bool mInitializated = false;
		private User32.RECT mOpenDialogWindowRect = new User32.RECT();
		private User32.RECT mOpenDialogClientRect = new User32.RECT();
		#endregion

		#region Constructors
		public OpenDialogNative(IntPtr handle, OpenFileDialogEx sourceControl)
		{
			mOpenDialogHandle = handle;
			mSourceControl = sourceControl;
			AssignHandle(mOpenDialogHandle);
		}
		#endregion

		#region Events
		private void BaseDialogNative_FileNameChanged(BaseDialogNative sender, string filePath)
		{
			if (mSourceControl != null)
				mSourceControl.OnFileNameChanged(filePath);
		}

		private void BaseDialogNative_FolderNameChanged(BaseDialogNative sender, string folderName)
		{
			if (mSourceControl != null)
				mSourceControl.OnFolderNameChanged(folderName);
		}
		#endregion

		#region Methods
		public void Dispose()
		{
			ReleaseHandle();
			if (mBaseDialogNative != null)
			{
				mBaseDialogNative.FileNameChanged -= new BaseDialogNative.PathChangedHandler(BaseDialogNative_FileNameChanged);
				mBaseDialogNative.FolderNameChanged -= new BaseDialogNative.PathChangedHandler(BaseDialogNative_FolderNameChanged);
				mBaseDialogNative.Dispose();
			}
		}
		#endregion

		#region Private Methods
		private void PopulateWindowsHandlers()
		{
			User32.EnumChildWindows(mOpenDialogHandle, 
				new User32.EnumWindowsCallBack(OpenFileDialogEnumWindowCallBack), 0);
		}

		private bool OpenFileDialogEnumWindowCallBack(IntPtr hwnd, int lParam)
		{
			StringBuilder className = new StringBuilder(256);
			User32.GetClassName(hwnd, className, className.Capacity);
			int controlID = User32.GetDlgCtrlID(hwnd);
			User32.WINDOWINFO windowInfo;
			User32.GetWindowInfo(hwnd, out windowInfo);

			// Dialog Window
			if (className.ToString().StartsWith("#32770"))
			{
				mBaseDialogNative = new BaseDialogNative(hwnd);
				mBaseDialogNative.FileNameChanged += new BaseDialogNative.PathChangedHandler(BaseDialogNative_FileNameChanged);
				mBaseDialogNative.FolderNameChanged += new BaseDialogNative.PathChangedHandler(BaseDialogNative_FolderNameChanged);
				return true;
			}

			switch ((ControlsID)controlID)
			{
				case ControlsID.DefaultView:
					mListViewPtr = hwnd;
					User32.GetWindowInfo(hwnd, out mListViewInfo);
					if (mSourceControl.DefaultViewMode != FolderViewMode.Default)
						User32.SendMessage(mListViewPtr, WindowMessage.WM_COMMAND, (IntPtr)mSourceControl.DefaultViewMode, IntPtr.Zero);
					break;
				case ControlsID.ComboFolder:
					mComboFolders = hwnd;
					mComboFoldersInfo = windowInfo;
					break;
				case ControlsID.ComboFileType:
					mComboExtensions = hwnd;
					mComboExtensionsInfo = windowInfo;
					break;
				case ControlsID.ComboFileName:
					if (className.ToString().ToLower() == "comboboxex32")
					{
						mComboFileName = hwnd;
						mComboFileNameInfo = windowInfo;
					}
					break;
				case ControlsID.GroupFolder:
					mGroupButtons = hwnd;
					mGroupButtonsInfo = windowInfo;
					break;
				case ControlsID.LeftToolBar:
					mToolBarFolders = hwnd;
					mToolBarFoldersInfo = windowInfo;
					break;
				case ControlsID.ButtonOpen:
					mOpenButton = hwnd;
					mOpenButtonInfo = windowInfo;
					break;
				case ControlsID.ButtonCancel:
					mCancelButton = hwnd;
					mCancelButtonInfo = windowInfo;
					break;
				case ControlsID.ButtonHelp:
					mHelpButton = hwnd;
					mHelpButtonInfo = windowInfo;
					break;
				case ControlsID.CheckBoxReadOnly:
					mChkReadOnly = hwnd;
					mChkReadOnlyInfo = windowInfo;
					break;
				case ControlsID.LabelFileName:
					mLabelFileName = hwnd;
					mLabelFileNameInfo = windowInfo;
					break;
				case ControlsID.LabelFileType:
					mLabelFileType = hwnd;
					mLabelFileTypeInfo = windowInfo;
					break;
			}

			return true;
		}

		private void InitControls()
		{
			mInitializated = true;

			// Lets get information about the current open dialog
			User32.GetClientRect(mOpenDialogHandle, ref mOpenDialogClientRect);
			User32.GetWindowRect(mOpenDialogHandle, ref mOpenDialogWindowRect);

			// Lets borrow the Handles from the open dialog control
			PopulateWindowsHandlers();

			User32.POINT pt = new User32.POINT();
			pt.X = mListViewInfo.rcWindow.left;
			pt.Y = mListViewInfo.rcWindow.top;
			User32.ScreenToClient(mOpenDialogHandle, ref pt);

			switch (mSourceControl.StartLocation)
			{
				case AddonWindowLocation.Right:
					// Now we transfer the control to the open dialog
					mSourceControl.Location = new Point(
						(int)(mOpenDialogClientRect.right - mSourceControl.Width - 6),
						pt.Y
						);
					break;
				case AddonWindowLocation.Bottom:
					// Now we transfer the control to the open dialog
					mSourceControl.Location = new Point(0, (int)(mOpenDialogClientRect.bottom - mSourceControl.Height));
					break;
				case AddonWindowLocation.None:
					// We don't have to do too much in this case, just the default thing
					mSourceControl.Location = new Point(0, 0);
					break;
			}
			// Everything is ready, now lets change the parent
			User32.SetParent(mSourceControl.Handle, mOpenDialogHandle);

			// Send the control to the back
			User32.SetWindowPos(mSourceControl.Handle, (IntPtr)ZOrderPos.HWND_BOTTOM, 0, 0, 0, 0, (int)SetWindowPosFlags.UFLAGSZORDER);
		}
		#endregion

		#region Overrides
		protected override void WndProc(ref Message m)
		{
			//Console.WriteLine(m.ToString());
			switch (m.Msg)
			{
				case (int)WindowMessage.WM_SHOWWINDOW:
					mInitializated = true;
					InitControls();
					SetControlPos();
					break;
				case (int)WindowMessage.WM_SIZING:
					User32.RECT currentSize;
					switch (mSourceControl.StartLocation)
					{
						case AddonWindowLocation.Right:

							currentSize = new User32.RECT();
							User32.GetWindowRect(mListViewPtr, ref currentSize);

							User32.POINT pt = new User32.POINT();
							pt.X = currentSize.right;
							pt.Y = currentSize.bottom;
							User32.ScreenToClient(mOpenDialogHandle, ref pt);

							if (currentSize.bottom != mSourceControl.Top + mSourceControl.Height)
								User32.SetWindowPos(
									mSourceControl.Handle, 
									(IntPtr)ZOrderPos.HWND_BOTTOM, 
									0, 
									0, 
									(int)mSourceControl.Width,
									(int)pt.Y - mSourceControl.Top, 
									(int)SetWindowPosFlags.UFLAGSSIZEEX);
							break;

						case AddonWindowLocation.Bottom:
							currentSize = new User32.RECT();
							User32.GetClientRect(mOpenDialogHandle, ref currentSize);
							if (currentSize.bottom != mSourceControl.Height)
								User32.SetWindowPos(mSourceControl.Handle, (IntPtr)ZOrderPos.HWND_BOTTOM, 0, 0, (int)currentSize.right, (int)mSourceControl.Height, (int)SetWindowPosFlags.UFLAGSSIZEEX);
							break;
						case AddonWindowLocation.None:
							currentSize = new User32.RECT();
							User32.GetClientRect(mOpenDialogHandle, ref currentSize);
							if (currentSize.right != mSourceControl.Width || currentSize.bottom != mSourceControl.Height)
								User32.SetWindowPos(mSourceControl.Handle, (IntPtr)ZOrderPos.HWND_BOTTOM, 0, 0, (int)currentSize.right, (int)currentSize.bottom, (int)SetWindowPosFlags.UFLAGSSIZEEX);
							break;
					}
					break;
				case (int)WindowMessage.WM_WINDOWPOSCHANGING:
					if (!mIsClosing)
					{
						if (!mInitializated)
						{
							// Resize OpenDialog to make fit our extra form
							User32.WINDOWPOS pos = (User32.WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(User32.WINDOWPOS));
							if (mSourceControl.StartLocation == AddonWindowLocation.Right)
							{
								if (pos.flags != 0 && ((pos.flags & (int)SetWindowPosFlags.SWP_NOSIZE) != (int)SetWindowPosFlags.SWP_NOSIZE))
								{
									PopulateWindowsHandlers();
									mOriginalSize = new Size(pos.cx, pos.cy);

									pos.cx += mSourceControl.Width + 6;
									Marshal.StructureToPtr(pos, m.LParam, true);

									SetControlPos();
								}
							}

							if (mSourceControl.StartLocation == AddonWindowLocation.Bottom)
							{
								if (pos.flags != 0 && ((pos.flags & (int)SetWindowPosFlags.SWP_NOSIZE) != (int)SetWindowPosFlags.SWP_NOSIZE))
								{
									mOriginalSize = new Size(pos.cx, pos.cy);

									pos.cy += mSourceControl.Height;
									Marshal.StructureToPtr(pos, m.LParam, true);

									SetControlPos();
								}
							}
						}
					}
					break;
				case (int)WindowMessage.WM_IME_NOTIFY:
					if (m.WParam == (IntPtr)ImeNotify.IMN_CLOSESTATUSWINDOW)
					{
						mIsClosing = true;
						mSourceControl.OnClosingDialog();

						User32.SetWindowPos(mOpenDialogHandle, IntPtr.Zero, 0, 0, 0, 0, (int)SetWindowPosFlags.UFLAGSHIDE);
						User32.GetWindowRect(mOpenDialogHandle, ref mOpenDialogWindowRect);
						User32.SetWindowPos(mOpenDialogHandle, IntPtr.Zero,
							(int)(mOpenDialogWindowRect.left),
							(int)(mOpenDialogWindowRect.top),
							(int)(mOriginalSize.Width),
							(int)(mOriginalSize.Height),
							(int)SetWindowPosFlags.UFLAGSSIZE);
					}
					break;
			}
			base.WndProc(ref m);
		}

		private void SetControlPos()
		{
			if (mSourceControl.StartLocation == AddonWindowLocation.Right)
			{
				var currentSize = new User32.RECT();
				User32.GetWindowRect(mListViewPtr, ref currentSize);

				User32.POINT pt = new User32.POINT();
				pt.X = currentSize.right;
				pt.Y = currentSize.bottom;
				User32.ScreenToClient(mOpenDialogHandle, ref pt);

				mSourceControl.Height = (int)pt.Y - mSourceControl.Top;
			}
			else if (mSourceControl.StartLocation == AddonWindowLocation.Bottom)
			{
				var currentSize = new User32.RECT();
				User32.GetClientRect(mOpenDialogHandle, ref currentSize);
				mSourceControl.Width = (int)currentSize.right;
			}
		}
		#endregion
	}

}

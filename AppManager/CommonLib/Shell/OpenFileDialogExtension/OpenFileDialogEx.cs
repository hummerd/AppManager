//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommonLib.PInvoke;


namespace CommonLib.Shell.OpenFileDialogExtension
{
    public partial class OpenFileDialogEx : UserControl
    {
        public delegate void PathChangedHandler(OpenFileDialogEx sender, string filePath);

        public event PathChangedHandler FileNameChanged;
        public event PathChangedHandler FolderNameChanged;
        public event EventHandler       ClosingDialog;


        private AddonWindowLocation mStartLocation  = AddonWindowLocation.Right;
        private FolderViewMode      mDefaultViewMode= FolderViewMode.Default;
		private DialogHostForm		mHostForm;

        public OpenFileDialogEx()
        {
            InitializeComponent();
			//OpenDialog.AutoUpgradeEnabled = false;
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        public OpenFileDialog OpenDialog
        {
            get {return dlgOpen;}
        }

        [DefaultValue(AddonWindowLocation.Right)]
        public AddonWindowLocation StartLocation
        {
            get {return mStartLocation;}
            set {mStartLocation = value;}
        }

        [DefaultValue(FolderViewMode.Default)]
        public FolderViewMode DefaultViewMode
        {
            get {return mDefaultViewMode;}
            set {mDefaultViewMode = value;}
        }

        #region Virtuals
        public virtual void OnFileNameChanged(string fileName)
        {
            if (FileNameChanged != null)
                FileNameChanged(this, fileName);
        }

        public virtual void OnFolderNameChanged(string folderName)
        {
            if (FolderNameChanged != null)
                FolderNameChanged(this, folderName);
        }

        public virtual void OnClosingDialog()
        {
            if (ClosingDialog != null)
                ClosingDialog(this, new EventArgs());
        }
        #endregion

        #region Methods
        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult returnDialogResult = DialogResult.Cancel;
			mHostForm = new DialogHostForm(this);
			mHostForm.Show(owner);
			User32.SetWindowPos(mHostForm.Handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.UFLAGSHIDE);
			mHostForm.WatchForActivate = true;
            try
            {
				returnDialogResult = dlgOpen.ShowDialog(mHostForm);
            }
            // Sometimes if you open a animated .gif on the preview and the Form is closed, .Net class throw an exception
            // Lets ignore this exception and keep closing the form.
            catch(Exception){}
			mHostForm.Close();
            return returnDialogResult;
        }

		public void CloseDialog(bool ok)
		{
			User32.EndDialog(mHostForm.OpenDialogHandle, ok ? 1 : 0);
		}

        #endregion
    }

	public enum FolderViewMode
	{
		Default = 0x7028,
		Icon = Default + 1,
		SmallIcon = Default + 2,
		List = Default + 3,
		Details = Default + 4,
		Thumbnails = Default + 5,
		Title = Default + 6,
		Thumbstrip = Default + 7,
	}

    public enum AddonWindowLocation
    {
        None    = 0,
        Right   = 1,
        Bottom  = 2
    }

    public enum ControlsID
    {
        ButtonOpen	    = 0x1,
        ButtonCancel	= 0x2,
        ButtonHelp	    = 0x40E,
        GroupFolder     = 0x440,
        LabelFileType   = 0x441,
        LabelFileName   = 0x442,
        LabelLookIn     = 0x443,
        DefaultView     = 0x461,
        LeftToolBar     = 0x4A0,
        ComboFileName   = 0x47c,
        ComboFileType   = 0x470,
        ComboFolder     = 0x471,
        CheckBoxReadOnly= 0x410
    }
 }
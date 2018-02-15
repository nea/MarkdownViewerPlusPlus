// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
//
// This file should stay in sync with the CPP project file
// "notepad-plus-plus/PowerEditor/src/WinControls/DockingWnd/Docking.h"
// found at
// https://github.com/notepad-plus-plus/notepad-plus-plus/blob/master/PowerEditor/src/WinControls/DockingWnd/Docking.h

using System;
using System.Runtime.InteropServices;

namespace Kbg.NppPluginNET.PluginInfrastructure
{

    [Flags]
    public enum NppTbMsg : uint
    {
        // styles for containers
        //CAPTION_TOP                = 1,
        //CAPTION_BOTTOM            = 0,

        // defines for docking manager
        CONT_LEFT = 0,
        CONT_RIGHT = 1,
        CONT_TOP = 2,
        CONT_BOTTOM = 3,
        DOCKCONT_MAX = 4,

        // mask params for plugins of internal dialogs
        DWS_ICONTAB = 0x00000001,            // Icon for tabs are available
        DWS_ICONBAR = 0x00000002,            // Icon for icon bar are available (currently not supported)
        DWS_ADDINFO = 0x00000004,            // Additional information are in use
        DWS_PARAMSALL = (DWS_ICONTAB | DWS_ICONBAR | DWS_ADDINFO),

        // default docking values for first call of plugin
        DWS_DF_CONT_LEFT = (CONT_LEFT << 28),    // default docking on left
        DWS_DF_CONT_RIGHT = (CONT_RIGHT << 28),    // default docking on right
        DWS_DF_CONT_TOP = (CONT_TOP << 28),        // default docking on top
        DWS_DF_CONT_BOTTOM = (CONT_BOTTOM << 28),    // default docking on bottom
        DWS_DF_FLOATING = 0x80000000            // default state is floating
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NppTbData
    {
        public IntPtr hClient;            // HWND: client Window Handle
        public string pszName;            // TCHAR*: name of plugin (shown in window)
        public int dlgID;                // int: a funcItem provides the function pointer to start a dialog. Please parse here these ID
                                         // user modifications
        public NppTbMsg uMask;                // UINT: mask params: look to above defines
        public uint hIconTab;            // HICON: icon for tabs
        public string pszAddInfo;        // TCHAR*: for plugin to display additional informations
                                         // internal data, do not use !!!
        public RECT rcFloat;            // RECT: floating position
        public int iPrevCont;           // int: stores the privious container (toggling between float and dock)
        public string pszModuleName;    // const TCHAR*: it's the plugin file name. It's used to identify the plugin
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public RECT(int left, int top, int right, int bottom)
        {
            Left = left; Top = top; Right = right; Bottom = bottom;
        }
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

}

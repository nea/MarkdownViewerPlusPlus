// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    public class ClikeStringArray : IDisposable
    {
        IntPtr _nativeArray;
        List<IntPtr> _nativeItems;
        bool _disposed = false;

        public ClikeStringArray(int num, int stringCapacity)
        {
            _nativeArray = Marshal.AllocHGlobal((num + 1) * IntPtr.Size);
            _nativeItems = new List<IntPtr>();
            for (int i = 0; i < num; i++)
            {
                IntPtr item = Marshal.AllocHGlobal(stringCapacity);
                Marshal.WriteIntPtr((IntPtr)((int)_nativeArray + (i * IntPtr.Size)), item);
                _nativeItems.Add(item);
            }
            Marshal.WriteIntPtr((IntPtr)((int)_nativeArray + (num * IntPtr.Size)), IntPtr.Zero);
        }
        public ClikeStringArray(List<string> lstStrings)
        {
            _nativeArray = Marshal.AllocHGlobal((lstStrings.Count + 1) * IntPtr.Size);
            _nativeItems = new List<IntPtr>();
            for (int i = 0; i < lstStrings.Count; i++)
            {
                IntPtr item = Marshal.StringToHGlobalUni(lstStrings[i]);
                Marshal.WriteIntPtr((IntPtr)((int)_nativeArray + (i * IntPtr.Size)), item);
                _nativeItems.Add(item);
            }
            Marshal.WriteIntPtr((IntPtr)((int)_nativeArray + (lstStrings.Count * IntPtr.Size)), IntPtr.Zero);
        }

        public IntPtr NativePointer { get { return _nativeArray; } }
        public List<string> ManagedStringsAnsi { get { return _getManagedItems(false); } }
        public List<string> ManagedStringsUnicode { get { return _getManagedItems(true); } }
        List<string> _getManagedItems(bool unicode)
        {
            List<string> _managedItems = new List<string>();
            for (int i = 0; i < _nativeItems.Count; i++)
            {
                if (unicode) _managedItems.Add(Marshal.PtrToStringUni(_nativeItems[i]));
                else _managedItems.Add(Marshal.PtrToStringAnsi(_nativeItems[i]));
            }
            return _managedItems;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                for (int i = 0; i < _nativeItems.Count; i++)
                    if (_nativeItems[i] != IntPtr.Zero) Marshal.FreeHGlobal(_nativeItems[i]);
                if (_nativeArray != IntPtr.Zero) Marshal.FreeHGlobal(_nativeArray);
                _disposed = true;
            }
        }
        ~ClikeStringArray()
        {
            Dispose();
        }
    }
}
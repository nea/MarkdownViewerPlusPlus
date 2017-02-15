// NPP plugin platform for .Net v0.93.96 by Kasper B. Graversen etc.
using System;
using System.Runtime.InteropServices;

namespace NppPlugin.DllExport
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    partial class DllExportAttribute : Attribute
    {
        public DllExportAttribute()
        {
        }

        public DllExportAttribute(string exportName)
            : this(exportName, CallingConvention.StdCall)
        {
        }

        public DllExportAttribute(string exportName, CallingConvention callingConvention)
        {
            ExportName = exportName;
            CallingConvention = callingConvention;
        }

        public CallingConvention CallingConvention { get; set; }

        public string ExportName { get; set; }
    }
}
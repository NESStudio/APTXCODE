using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;

namespace Aptxcode
{
    sealed class CHConvert
    {
        private CHConvert()
        { }

        public static string MapString(CultureInfo ci, uint mapFlags, string srcStr)
        {
            StringBuilder lpDestStr = new StringBuilder(srcStr.Length);
            if (NativeMethods.LCMapString((uint)ci.LCID, mapFlags, srcStr, srcStr.Length, lpDestStr, srcStr.Length) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            else
                return lpDestStr.ToString();
        }


        public static string MapString(uint mapFlags, string source)
        {
            return MapString(CurrentThreadCultureInfo(), mapFlags, source);
        }
        private static CultureInfo CurrentThreadCultureInfo()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        public static string ConvChsToCht(string srcStr)
        {
            return MapString(new CultureInfo("zh-CN", false), NativeMethods.LCMAP_TRADITIONAL_CHINESE, srcStr);
        }

    }
}

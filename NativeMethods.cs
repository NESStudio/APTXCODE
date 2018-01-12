using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aptxcode
{
    static class NativeMethods
    {
        #region String mapping flags
        /// <summary>
        /// lower case letters
        /// </summary>
        internal const uint LCMAP_LOWERCASE = 0x00000100;
        /// <summary>
        /// upper case letters
        /// </summary>
        internal const uint LCMAP_UPPERCASE = 0x00000200;
        /// <summary>
        /// WC sort key (normalize)
        /// </summary>
        internal const uint LCMAP_SORTKEY = 0x00000400;
        /// <summary>
        /// byte reversal
        /// </summary>
        internal const uint LCMAP_BYTEREV = 0x00000800;
        /// <summary>
        /// map katakana to hiragana
        /// </summary>
        internal const uint LCMAP_HIRAGANA = 0x00100000;
        /// <summary>
        /// map hiragana to katakana
        /// </summary>
        internal const uint LCMAP_KATAKANA = 0x00200000;
        /// <summary>
        /// map double byte to single byte
        /// </summary>
        internal const uint LCMAP_HALFWIDTH = 0x00400000;
        /// <summary>
        /// map single byte to double byte
        /// </summary>
        internal const uint LCMAP_FULLWIDTH = 0x00800000;
        /// <summary>
        /// use linguistic rules for casing
        /// </summary>
        internal const uint LCMAP_LINGUISTIC_CASING = 0x01000000;
        /// <summary>
        /// map traditional chinese to simplified chinese
        /// </summary>
        internal const uint LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        /// <summary>
        /// map simplified chinese to traditional chinese
        /// </summary>
        internal const uint LCMAP_TRADITIONAL_CHINESE = 0x04000000;
        /// <summary>
        /// ignore case, used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint NORM_IGNORECASE = 0x00000001;
        /// <summary>
        /// ignore nonspacing chars,can be used alone, with one another, or with the LCMAP_SORTKEY and/or LCMAP_BYTEREV flags. 
        /// </summary>
        internal const uint NORM_IGNORENONSPACE = 0x00000002;
        /// <summary>
        /// ignore symbols, can be used alone, with one another, or with the LCMAP_SORTKEY and/or LCMAP_BYTEREV flags. 
        /// </summary>
        internal const uint NORM_IGNORESYMBOLS = 0x00000004;
        /// <summary>
        /// linguistically appropriate 'ignore case' , used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint LINGUISTIC_IGNORECASE = 0x00000010;
        /// <summary>
        /// linguistically appropriate 'ignore nonspace' , used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint LINGUISTIC_IGNOREDIACRITIC = 0x00000020;
        /// <summary>
        /// ignore kanatype, used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint NORM_IGNOREKANATYPE = 0x00010000;
        /// <summary>
        /// ignore width, used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint NORM_IGNOREWIDTH = 0x00020000;
        /// <summary>
        /// use linguistic rules for casing, used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint NORM_LINGUISTIC_CASING = 0x08000000;
        /// <summary>
        /// use string sort method, used only with the LCMAP_SORTKEY flag.
        /// </summary>
        internal const uint SORT_STRINGSORT = 0x00001000;
        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int LCMapString(
            [In]uint Locale,
            [In]uint dwMapFlags,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpSrcStr,
            [In]int cchSrc,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpDestStr,
            [In]int cchDest
            );
    }
}

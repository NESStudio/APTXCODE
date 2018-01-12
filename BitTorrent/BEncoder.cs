using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aptxcode.BitTorrent
{
    static class BEncoder
    {
        public static byte[] BEncode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] tmp = Encoding.UTF8.GetBytes(str);
                byte[] length = Encoding.ASCII.GetBytes(tmp.Length.ToString() + ":");
                return length.Concat(tmp).ToArray();
            }
            else
                throw new ArgumentNullException("str", "Can't Encode null or empty string!");
        }

        public static byte[] BEncode(byte[] binstr)
        {
            if (binstr.Length % 20 == 0)
            {
                byte[] length = Encoding.ASCII.GetBytes(binstr.Length.ToString() + ":");
                return length.Concat(binstr).ToArray();
            }
            else
                throw new ArgumentException("Error data!");
        }

        public static byte[] BEncode(long integer)
        {
            return Encoding.ASCII.GetBytes(string.Format("i{0}e", integer));
        }

        public static byte[] BEncode(ArrayList list)
        {
            if (list.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.WriteByte((byte)'l');
                    foreach (var item in list)
                    {
                        byte[] tmp = BEncode(item);
                        ms.Write(tmp, 0, tmp.Length);
                    }
                    ms.WriteByte((byte)'e');
                    return ms.ToArray();
                }
            }
            else
                throw new ArgumentNullException("list", "List can't be null!");
        }

        public static byte[] BEncode(SortedDictionary<string, object> dictionarie)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte((byte)'d');
                foreach (KeyValuePair<string, object> pair in dictionarie)
                {
                    byte[] key = BEncode(pair.Key);
                    byte[] value = BEncode(pair.Value);
                    ms.Write(key, 0, key.Length);
                    ms.Write(value, 0, value.Length);
                }
                ms.WriteByte((byte)'e');
                return ms.ToArray();
            }
        }

        public static byte[] BEncode(object obj)
        {
            string str = obj as string;
            if (str != null)
            {
                return BEncode(str);
            }
            else
            {
                ArrayList list = obj as ArrayList;
                if (list != null)
                {
                    return BEncode(list);
                }
                else
                {
                    byte[] bytes = obj as byte[];
                    if (bytes != null)
                    {
                        return BEncode(bytes);
                    }
                    else
                    {
                        SortedDictionary<string, object> sd = obj as SortedDictionary<string, object>;
                        if (sd != null)
                        {
                            return BEncode(sd);
                        }
                        else if (obj is int)
                        {
                            return BEncode((long)(int)obj);
                        }
                        else if (obj is long)
                        {
                            return BEncode((long)obj);
                        }
                        else
                        {
                            throw new ArgumentException("Unsupported type");
                        }
                    }
                }
            }
        }
    }
}

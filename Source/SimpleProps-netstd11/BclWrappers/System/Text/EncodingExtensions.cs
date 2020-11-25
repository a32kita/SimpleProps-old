using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps.BclWrappers.System.Text
{
    public static class EncodingExtensions
    {
        public static string GetString(this Encoding encoding, byte[] bytes)
        {
            return encoding.GetString(bytes, 0, bytes.Length);
        }
    }
}

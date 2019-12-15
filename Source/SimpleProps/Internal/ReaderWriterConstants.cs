using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps.Internal
{
    internal static class ReaderWriterConstants
    {
        public static readonly Encoding DefaultEncoding;

        static ReaderWriterConstants()
        {
            DefaultEncoding = Encoding.UTF8;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps.Internal
{
    internal enum PropItemBufferMode : byte
    {
        Null = 0,

        Buffered = 1,

        GzipBuffered = 2,

        Reference = 3,
    }
}

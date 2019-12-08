using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps
{
    public enum PropType : ushort
    {
        Int16 = 1,

        Int32 = 2,

        Int64 = 3,

        String = 11,

        InversedString = 12,

        Int16Array = 31,

        Int32Array = 32,

        Int64Array = 33,

        Buffer = 51,
    }
}

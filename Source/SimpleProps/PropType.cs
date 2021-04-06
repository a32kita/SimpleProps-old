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

        Double = 4,

        String = 11,

        InversedString = 12,
        
        DateTime = 16,

        Guid = 17,

        Int16Array = 31,

        Int32Array = 32,

        Int64Array = 33,

        DoubleArray = 34,

        StringArray = 41,

        Buffer = 51,
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps
{
    public static class PropTypeUtils
    {
        public static PropType GetType(Object obj)
        {
            if (obj is Int16)
                return PropType.Int16;
            if (obj is Int32)
                return PropType.Int32;
            if (obj is Int64)
                return PropType.Int64;
            if (obj is String)
                return PropType.String;
            if (obj is DateTime)
                return PropType.DateTime;
            if (obj is Int16[])
                return PropType.Int16Array;
            if (obj is Int32[])
                return PropType.Int32Array;
            if (obj is Int64[])
                return PropType.Int64Array;
            if (obj is Byte[])
                return PropType.Buffer;

            throw new ArgumentException();
        }

        public static bool CheckType(Object obj, PropType type)
        {
            if (obj == null)
                return true;

            if (obj is String && type == PropType.InversedString)
                return true;

            try
            {
                var actualType = GetType(obj);
                return type == actualType;
            }
            catch
            {
                return false;
            }
        }
    }
}

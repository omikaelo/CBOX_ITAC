using System;
using System.Globalization;

namespace Itac4Lear
{
    public static class GenericFunctions
    {
        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString((IFormatProvider)new CultureInfo("en-US"));
        }
    }
}

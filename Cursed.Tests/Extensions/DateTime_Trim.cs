using System;

namespace Cursed.Tests.Extensions
{
    public static partial class Extension
    {
        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind);
        }
    }
}

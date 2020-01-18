using System;

namespace Cursed.Models.Extensions
{
    /// <summary>
    /// Trim extensions for DateTime struct. Clear value of instance up to selected precision.
    /// </summary>
    public static partial class DateTimeExtensions
    {
        /// <summary>
        /// Trim given DateTime with specific precision
        /// </summary>
        /// <param name="roundTicks">Precision given in ticks format. Can be used TimeSpan. TicksPerX const</param>
        /// <returns>Trimmed DateTime</returns>
        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind);
        }

        /// <summary>
        /// Trim given DateTime keeping days and lager units
        /// </summary>
        /// <returns>Trimmed DateTime</returns>
        public static DateTime TrimUpToDays(this DateTime date)
        {
            return date.Trim(TimeSpan.TicksPerDay);
        }
    }
}

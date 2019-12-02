using System;
using Cursed.Models.Extensions;

namespace Cursed.Models.Data.Licenses
{
    public class LicensesDataModel : LicensesAbstractModel
    {
        private DateTime _date;
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
    }
}

using System;
using Cursed.Models.Extensions;

namespace Cursed.Models.DataModel.Licenses
{
    /// <summary>
    /// Model used as base for licenses data gathering 
    /// </summary>
    public class LicensesDataModel : LicensesAbstractModel
    {
        private DateTime _date;
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
    }
}

using System;
using Cursed.Models.Extensions;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class License
    {
        private DateTime _date;

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int GovermentNum { get; set; }
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
        

        public virtual ProductCatalog Product { get; set; }
    }
}

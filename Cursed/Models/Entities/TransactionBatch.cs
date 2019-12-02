using System;
using System.Collections.Generic;
using Cursed.Models.Extensions;

namespace Cursed.Models.Entities
{
    public partial class TransactionBatch
    {
        public TransactionBatch()
        {
            Operation = new HashSet<Operation>();
        }

        private DateTime _date;

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
        public string Type { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Operation> Operation { get; set; }
    }
}

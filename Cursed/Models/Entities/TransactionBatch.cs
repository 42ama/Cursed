using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class TransactionBatch
    {
        public TransactionBatch()
        {
            Operation = new HashSet<Operation>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Operation> Operation { get; set; }
    }
}

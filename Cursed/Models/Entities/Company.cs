using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class Company
    {
        public Company()
        {
            Storage = new HashSet<Storage>();
            TransactionBatch = new HashSet<TransactionBatch>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Storage> Storage { get; set; }
        public virtual ICollection<TransactionBatch> TransactionBatch { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class Company
    {
        public Company()
        {
            Storage = new HashSet<Storage>();
            TransactionBatch = new HashSet<TransactionBatch>();
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Storage> Storage { get; set; }
        public virtual ICollection<TransactionBatch> TransactionBatch { get; set; }
    }
}

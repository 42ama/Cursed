using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cursed.Models.Extensions;

namespace Cursed.Models.Entities.Data
{
    public partial class TransactionBatch
    {
        public TransactionBatch()
        {
            Operation = new HashSet<Operation>();
        }

        private DateTime _date;

        [Required]
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
        [Required]
        public string Type { get; set; }
        [Required]
        public bool IsOpen { get; set; } = true;
        public string Comment { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Operation> Operation { get; set; }
    }
}

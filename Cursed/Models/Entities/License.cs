using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class License
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int GovermentNum { get; set; }
        public DateTime Date { get; set; }

        public virtual ProductCatalog Product { get; set; }
    }
}

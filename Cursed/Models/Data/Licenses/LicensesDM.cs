using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Licenses
{
    public class LicensesDM
    {
        public int Id { get; set; }
        public int GovermentNum { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? ProductCAS { get; set; }
    }
}

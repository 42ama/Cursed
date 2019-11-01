using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Licenses
{
    public class LicensesVM
    {
        public int Id { get; set; }
        public int GovermentNum { get; set; }
        public string Date { get; set; }
        public bool IsValid { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCAS { get; set; }
    }
}

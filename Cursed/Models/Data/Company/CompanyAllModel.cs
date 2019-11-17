using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Company
{
    public class CompanyAllModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StoragesCount { get; set; }
        public int TransactionsCount { get; set; }
    }
}

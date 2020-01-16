using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.Companies
{
    public class CompaniesModel : CompaniesAbstractModel
    {
        public int StoragesCount { get; set; }
        public int TransactionsCount { get; set; }
    }
}

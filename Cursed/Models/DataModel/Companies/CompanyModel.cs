using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.DataModel.Utility;

namespace Cursed.Models.DataModel.Companies
{
    public class CompanyModel : CompaniesAbstractModel
    {
        public List<ValueTuple<string, int>> Storages { get; set; }
        public List<ValueTuple<string, int>> Transactions { get; set; } //title is date string
    }
}

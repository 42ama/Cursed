using System;
using System.Collections.Generic;

namespace Cursed.Models.DataModel.Companies
{
    /// <summary>
    /// Models used for single company data gathering
    /// </summary>
    public class CompanyModel : CompaniesAbstractModel
    {
        public List<ValueTuple<string, int>> Storages { get; set; }
        public List<ValueTuple<string, int>> Transactions { get; set; }
    }
}

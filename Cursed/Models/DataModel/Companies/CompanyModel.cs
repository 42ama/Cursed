using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.DataModel.Utility;

namespace Cursed.Models.DataModel.Companies
{
    public class CompanyModel : CompaniesAbstractModel
    {
        public List<TitleIdContainer> Storages { get; set; }
        public List<TitleIdContainer> Transactions { get; set; } //title is date string
    }
}

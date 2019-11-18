﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Data.Companies
{
    public class CompanyModel : CompaniesAbstractModel
    {
        public List<TitleIdContainer> Storages { get; set; }
        public List<TitleIdContainer> Transactions { get; set; } //title is date string
    }
}
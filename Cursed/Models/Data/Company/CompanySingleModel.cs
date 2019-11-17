using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Data.Company
{
    public class CompanySingleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TitleIdContainer> Storages { get; set; }
        public List<TitleIdContainer> Transactions { get; set; } //title is date string
    }
}

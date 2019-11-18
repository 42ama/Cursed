using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Data.Storages
{
    public class StoragesAbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TitleIdContainer Company { get; set; }
    }
}

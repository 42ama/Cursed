using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.DataModel.Utility;

namespace Cursed.Models.DataModel.Storages
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

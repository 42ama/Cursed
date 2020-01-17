﻿using Cursed.Models.DataModel.Products;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.Storages
{
    public class StorageModel : StoragesAbstractModel
    {
        public List<ProductContainer> Products { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Routing
{
    public class ProductsRouting
    {
        public const string Index = "GetProducts";
        public const string SingleItem = "GetProduct";
        public const string GetEditSingleItem = "GetProductForEdit";
        public const string AddSingleItem = "AddProduct";
        public const string EditSingleItem = "EditProduct";
        public const string DeleteSingleItem = "DeleteProduct";
    }
}

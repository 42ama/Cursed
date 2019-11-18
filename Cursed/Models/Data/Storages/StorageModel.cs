using Cursed.Models.Data.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Storages
{
    public class StorageModel : StoragesAbstractModel
    {
        public List<ProductContainer> Products { get; set; }
    }
}

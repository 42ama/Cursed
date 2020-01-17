using System;

namespace Cursed.Models.DataModel.Storages
{
    public class StoragesAbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public ValueTuple<string, int> Company { get; set; }
    }
}

using System;

namespace Cursed.Models.DataModel.Storages
{
    /// <summary>
    /// Model used as base for storages data gathering 
    /// </summary>
    public class StoragesAbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public ValueTuple<string, int> Company { get; set; }
    }
}

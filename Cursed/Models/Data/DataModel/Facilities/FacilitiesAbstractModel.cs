

namespace Cursed.Models.DataModel.Facilities
{
    /// <summary>
    /// Model used as base for facilities data gathering 
    /// </summary>
    public abstract class FacilitiesAbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}


namespace Cursed.Models.Data.Facilities
{
    public abstract class FacilitiesAbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}

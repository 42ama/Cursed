

namespace Cursed.Models.DataModel.Companies
{
    /// <summary>
    /// Model used as base for companies data gathering 
    /// </summary>
    public abstract class CompaniesAbstractModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

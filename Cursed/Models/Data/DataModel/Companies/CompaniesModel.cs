

namespace Cursed.Models.DataModel.Companies
{
    /// <summary>
    /// Model used for multiple companies data gathering
    /// </summary>
    public class CompaniesModel : CompaniesAbstractModel
    {
        public int StoragesCount { get; set; }
        public int TransactionsCount { get; set; }
    }
}

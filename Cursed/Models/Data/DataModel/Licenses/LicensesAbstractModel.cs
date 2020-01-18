

namespace Cursed.Models.DataModel.Licenses
{
    /// <summary>
    /// Model used as base for licenses data gathering 
    /// </summary>
    public abstract class LicensesAbstractModel
    {
        public int Id { get; set; }
        public int GovermentNum { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCAS { get; set; }
    }
}

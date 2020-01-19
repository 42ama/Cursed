

namespace Cursed.Models.DataModel.Transactions
{
    /// <summary>
    /// Model used for operations data gathering 
    /// </summary>
    public class OperationModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int? CAS { get; set; }
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string StorageFromName { get; set; }
        public int StorageFromId { get; set; }
        public string StorageToName { get; set; }
        public int StorageToId { get; set; }
    }
}

using System.Collections.Generic;

namespace Cursed.Models.DataModel.Transactions
{
    /// <summary>
    /// Model used for single transaction data gathering
    /// </summary>
    public class TransactionModel : TransactionsAbstractModel
    {
        public List<OperationModel> Operations { get; set; }
    }
}

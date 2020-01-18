

namespace Cursed.Models.StaticReferences.Routing
{
    /// <summary>
    /// Route names for Transactions section
    /// </summary>
    public class TransactionsRouting
    {
        public const string Index = "GetTransactions";
        public const string SingleItem = "GetTransaction";
        public const string GetEditSingleItem = "GetTransactionForEdit";
        public const string AddSingleItem = "AddTransaction";
        public const string EditSingleItem = "EditTransaction";
        public const string DeleteSingleItem = "DeleteTransaction";

        public const string CloseTransaction = "CloseTransaction";
        public const string OpenTransaction = "OpenTransaction";
    }
}

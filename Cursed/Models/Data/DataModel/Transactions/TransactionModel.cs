using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Entities;

namespace Cursed.Models.DataModel.Transactions
{
    public class TransactionModel : TransactionsAbstractModel
    {
        public List<OperationModel> Operations { get; set; }
    }
}

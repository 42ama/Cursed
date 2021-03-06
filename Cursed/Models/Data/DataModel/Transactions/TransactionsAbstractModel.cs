﻿using System;

namespace Cursed.Models.DataModel.Transactions
{
    /// <summary>
    /// Model used as base for transactions data gathering 
    /// </summary>
    public class TransactionsAbstractModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public bool IsOpen { get; set; }
        public string Comment { get; set; }
    }
}


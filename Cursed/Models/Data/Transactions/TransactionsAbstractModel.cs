﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Extensions;

namespace Cursed.Models.Data.Transactions
{
    public class TransactionsAbstractModel
    {
        private DateTime _date;
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
        public string Type { get; set; }
        public bool IsOpen { get; set; }
        public string Comment { get; set; }
    }
}
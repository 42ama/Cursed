using System;
using Cursed.Models.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class License
    {
        private DateTime _date;

        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Goverment number must be positive")]
        public int GovermentNum { get; set; }
        [Required]
        public DateTime Date { get { return _date; } set { _date = value.TrimUpToDays(); } }
        

        public virtual ProductCatalog Product { get; set; }
    }
}

using System;
using Cursed.Models.Entities;

namespace Cursed.Models.Data.Shared
{
    public class LicenseValid : License
    {
        public bool IsValid { get; set; }

        public LicenseValid(License license)
        {
            Id = license.Id;
            ProductId = license.ProductId;
            Date = license.Date;
            GovermentNum = license.GovermentNum;
            Product = license.Product;

            Validate();
        }

        private void Validate()
        {
            if (Date > DateTime.UtcNow)
            {
                IsValid = true;
            }
            else
            {
                IsValid = false;
            }
        }
    }
}

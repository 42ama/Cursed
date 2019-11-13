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

            IsValid = Validate(Date);
        }
        public LicenseValid(License license, bool isValid) : this(license)
        {
            IsValid = isValid;
        }

        public static bool Validate(DateTime date)
        {
            if (date > DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Validate(License license)
        {
            return Validate(license.Date);
        }
    }
}

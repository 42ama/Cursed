using System;
using Cursed.Models.Entities;

namespace Cursed.Models.Services
{
    public interface ILicenseValidation
    {
        bool IsValid(License license);
    }
}

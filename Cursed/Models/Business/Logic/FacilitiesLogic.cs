﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Facilities;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Services;

namespace Cursed.Models.Logic
{
    public class FacilitiesLogic : IReadColection<FacilitiesModel>, IReadSingle<FacilityModel>, IReadUpdateForm<Facility>, ICUD<Facility>
    {
        private readonly CursedDataContext db;
        private readonly ILicenseValidation licenseValidation;
        public FacilitiesLogic(CursedDataContext db, ILicenseValidation licenseValidation)
        {
            this.db = db;
            this.licenseValidation = licenseValidation;
        }

        public async Task<IEnumerable<FacilitiesModel>> GetAllDataModelAsync()
        {
            var facilities = await db.Facility.ToListAsync();
            var query = from f in facilities
                        join q in (from f in facilities
                                   join tp in db.TechProcess on f.Id equals tp.FacilityId into recipes
                                   from r in recipes.DefaultIfEmpty()
                                   group r by f.Id) on f.Id equals q.Key into techprocesses
                        from tab in techprocesses.DefaultIfEmpty()
                        select new FacilitiesModel
                        {
                            Id = f.Id,
                            Latitude = f.Latitude,
                            Longitude = f.Longitude,
                            Name = f.Name,
                            TechProcesses = tab.ToList()
                        };

            return query;
        }

        public async Task<FacilityModel> GetSingleDataModelAsync(object key)
        {
            var facilities = await db.Facility.ToListAsync();
            var licensesList = await db.License.ToListAsync();

            var queryInner = (from f in facilities
                             where f.Id == (int)key
                             join tp in db.TechProcess on f.Id equals tp.FacilityId into techprocesses
                             from tps in techprocesses.DefaultIfEmpty()
                             join r in db.Recipe on tps.RecipeId equals r.Id into recipes
                             from rs in recipes.DefaultIfEmpty()
                             join rpc in db.RecipeProductChanges on rs.Id equals rpc.RecipeId into recipeproducts
                             from rpcs in recipeproducts.DefaultIfEmpty()
                             join pc in db.ProductCatalog on rpcs.ProductId equals pc.Id into products
                             from pcs in products.DefaultIfEmpty()
                             group new FacilitiesProductContainer()
                             {
                                 FacilityId = f.Id,
                                 RecipeId = tps.RecipeId,
                                 RecipeEfficiency = tps.DayEfficiency,
                                 RecipeGovApprov = rs.GovermentApproval ?? false,
                                 RecipeTechnoApprov = rs.TechApproval ?? false,
                                 ProductId = rpcs.ProductId,
                                 ProductType = rpcs.Type,
                                 Quantity = rpcs.Quantity,
                                 ProductName = pcs.Name,
                                 LicenseRequired = pcs.LicenseRequired ?? false
                             } by f.Id).ToList();

            // for each product connect license from previously loaded list and set IsValid property,
            // baseed on validation
            foreach (var group in queryInner)
            {
                foreach (var item in group)
                {
                    if(item.LicenseRequired)
                    {
                        item.IsValid = false;
                        var licenses = licensesList.Where(i => i.ProductId == item.ProductId);
                        foreach (var license in licenses)
                        {
                            if (licenseValidation.IsValid(license))
                            {
                                item.IsValid = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        item.IsValid = true;
                    }
                }
            }

            var queryOuter = from f in facilities
                        where f.Id == (int)key
                        join qi in queryInner on f.Id equals qi.Key
                        select new FacilityModel
                        {
                            Id = f.Id,
                            Latitude = f.Latitude,
                            Longitude = f.Longitude,
                            Name = f.Name,
                            Products = qi.ToList()
                        };

            return queryOuter.Single();
        }


        public async Task<Facility> GetSingleUpdateModelAsync(object key)
        {
            return await db.Facility.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        public async Task<Facility> AddDataModelAsync(Facility model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task UpdateDataModelAsync(Facility model)
        {
            var currentModel = await db.Facility.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.Facility.FindAsync((int)key);
            db.Facility.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}
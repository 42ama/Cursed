﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Cursed.Models.Context;
using Cursed.Models.Data.Facilities;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Services;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class FacilitiesLogic// : IReadColection<FacilitiesModel>, IReadSingle<FacilityModel>, IReadUpdateForm<Facility>, ICUD<Facility>
    {
        private readonly CursedContext db;
        private readonly ILicenseValidation licenseValidation;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public FacilitiesLogic(CursedContext db, ILicenseValidation licenseValidation)
        {
            this.db = db;
            this.licenseValidation = licenseValidation;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler<IEnumerable<FacilitiesModel>>> GetAllDataModelAsync()
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<FacilitiesModel>>("All facilities.");

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

            statusMessage.ReturnValue = query;

            return statusMessage;
        }

        public async Task<AbstractErrorHandler<FacilityModel>> GetSingleDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<FacilityModel>("Facility.", key);

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
                                 RecipeEfficiency = tps.DayEffiency,
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

            statusMessage.ReturnValue = queryOuter.Single();

            return statusMessage;
        }


        public async Task<AbstractErrorHandler<Facility>> GetSingleUpdateModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<Facility>("Facility.", key);

            statusMessage.ReturnValue = await db.Facility.SingleOrDefaultAsync(i => i.Id == (int)key);

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> AddDataModelAsync(Facility model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Facility.");

            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> UpdateDataModelAsync(Facility model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Facility.", model.Id);

            var currentModel = await db.Facility.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> RemoveDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<Facility>("Facility.", key);

            var entity = await db.Facility.FindAsync((int)key);
            db.Facility.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

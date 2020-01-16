using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Cursed.Models.Context;
using Cursed.Models.Data.Recipes;
using Cursed.Models.Entities.Data;
using Cursed.Models.Data.Shared;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility.ErrorHandling;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Logic
{
    public class RecipesLogic : IReadColection<RecipesModel>, IReadSingle<RecipeModel>, IReadUpdateForm<Recipe>, ICUD<Recipe>
    {
        private readonly CursedDataContext db;
        public RecipesLogic(CursedDataContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<RecipesModel>> GetAllDataModelAsync()
        {
            var recipes = await db.Recipe.ToListAsync();
            var query = from r in recipes
                     join ak in (from r in recipes
                                 join ri in db.RecipeInheritance on r.Id equals ri.ChildId into t
                                 group t by r.Id) on r.Id equals ak.Key into parentIdSingle
                     from pis in parentIdSingle.DefaultIfEmpty()
                     join bk in (from r in recipes
                                 join ri in db.RecipeInheritance on r.Id equals ri.ParentId into t
                                 group t by r.Id) on r.Id equals bk.Key into childIds
                     from ci in childIds.DefaultIfEmpty()
                     join ck in (from rr in (from r in recipes
                                             join rpc in db.RecipeProductChanges on r.Id equals rpc.RecipeId into t
                                             from t2 in t
                                             join pc in db.ProductCatalog on t2?.ProductId equals pc.Id into t3
                                             from t4 in t3
                                             select new
                                             {
                                                 RecipeId = r.Id,
                                                 Type = t2.Type
                                             })
                                 group rr by rr.RecipeId) on r.Id equals ck.Key into products
                     from p in products.DefaultIfEmpty()
                     select new RecipesModel
                     {
                         Id = r.Id,
                         Content = r.Content,
                         TechApproved = r.TechApproval,
                         GovApproved = r.GovermentApproval,
                         ParentRecipe = pis.Single().SingleOrDefault()?.ParentId,
                         ChildRecipesCount = ci.Single().Select(i => i.ChildId).Count(),
                         ProductCount = p?.Select(i => i.Type).Where(i=>i==ProductCatalogTypes.Product).Count() ?? 0,
                         MaterialCount = p?.Select(i => i.Type).Where(i=> i == ProductCatalogTypes.Material).Count() ?? 0
                     };

            return query;
        }
        public async Task<RecipeModel> GetSingleDataModelAsync(object key)
        {
            var recipes = await db.Recipe.ToListAsync();
            var query = from r in recipes
                        where r.Id == (int)key
                        join ak in (from r in recipes
                                    join ri in db.RecipeInheritance on r.Id equals ri.ChildId into t
                                    group t by r.Id) on r.Id equals ak.Key into parentIdSingle
                        from pis in parentIdSingle.DefaultIfEmpty()
                        join bk in (from r in recipes
                                    join ri in db.RecipeInheritance on r.Id equals ri.ParentId into t
                                    group t by r.Id) on r.Id equals bk.Key into childIds
                        from ci in childIds.DefaultIfEmpty()
                        join tp in (from tps in (from r in recipes
                                               join tp in db.TechProcess on r.Id equals tp.RecipeId into t
                                               from t2 in t
                                               join f in db.Facility on t2.FacilityId equals f.Id into t3
                                               from fac in t3
                                               select new
                                               {
                                                   RecipeId = r.Id,
                                                   Facility = new Facility
                                                   {
                                                       Name = fac.Name,
                                                       Id = fac.Id
                                                   }
                                               })
                                    group tps by tps.RecipeId) on r.Id equals tp.Key into techProcesses
                        from techP in techProcesses.DefaultIfEmpty()
                        join ck in (from rr in (from r in recipes
                                                join rpc in db.RecipeProductChanges on r.Id equals rpc.RecipeId into t
                                                from t2 in t
                                                join pc in db.ProductCatalog on t2.ProductId equals pc.Id into t3
                                                from t4 in t3
                                                select new
                                                {
                                                    RecipeId = r.Id,
                                                    RecipeProductContainer = new RecipeProductContainer
                                                    { 
                                                        CAS = t4.Cas,
                                                        ProductId = t4.Id,
                                                        ProductName = t4.Name,
                                                        Quantity = t2.Quantity,
                                                        Type = t2.Type
                                                    }
                                                })
                                    group rr by rr.RecipeId) on r.Id equals ck.Key into products
                        from p in products.DefaultIfEmpty()
                        select new RecipeModel
                        {
                            Id = r.Id,
                            Content = r.Content,
                            TechApproved = r.TechApproval,
                            GovApproved = r.GovermentApproval,
                            ParentRecipe = pis.Single().SingleOrDefault()?.ParentId,
                            ChildRecipes = ci.Single().Select(i => i.ChildId).ToList(),
                            RecipeProducts = p?.Select(i => i.RecipeProductContainer).ToList() ?? new List<RecipeProductContainer>(),
                            RelatedFacilities = techP?.Select(i => i.Facility).ToList() ?? new List<Facility>()
                        };
            return query.Single();
        }
        public async Task<Recipe> GetSingleUpdateModelAsync(object key)
        {
            return await db.Recipe.SingleAsync(i => i.Id == (int)key);
        }
        public async Task<Recipe> AddDataModelAsync(Recipe model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }
        public async Task InverseTechnologistApprovalAsync(object key)
        {
            var model = await db.Recipe.FirstOrDefaultAsync(i => i.Id == (int)key);
            if(model.TechApproval == null)
            {
                model.TechApproval = true;
            }
            else
            {
                model.TechApproval = !model.TechApproval;
            }
            await UpdateDataModelAsync(model);
        }
        public async Task InverseGovermentApprovalAsync(object key)
        {
            var model = await db.Recipe.FirstOrDefaultAsync(i => i.Id == (int)key);
            if (model.GovermentApproval == null)
            {
                model.GovermentApproval = true;
            }
            else
            {
                model.GovermentApproval = !model.GovermentApproval;
            }
            await UpdateDataModelAsync(model);
        }
        public async Task<Recipe> AddChildDataModelAsync(Recipe model, int parentId)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();

            // copy all products to new recipe for later manual change
            var recipeProductChanges = db.RecipeProductChanges.Where(i => i.RecipeId == parentId).Select(i => new RecipeProductChanges 
            { 
                RecipeId = entity.Entity.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Type = i.Type
            });
            db.Add(new RecipeInheritance
            {
                ChildId = entity.Entity.Id,
                ParentId = parentId
            });
            db.RecipeProductChanges.AddRange(recipeProductChanges);
            
            await db.SaveChangesAsync();
            return entity.Entity;


        }
        public async Task UpdateDataModelAsync(Recipe model)
        {
            var currentModel = await db.Recipe.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }
        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.Recipe.FindAsync((int)key);
            var parentsInheritance = db.RecipeInheritance.Where(i => i.ChildId == (int)key);
            db.RecipeInheritance.RemoveRange(parentsInheritance);
            db.Recipe.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

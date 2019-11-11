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
using Cursed.Models.Data.Recipe;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Logic
{
    public class RecipeLogic : IRESTAsync<RecipeAllModel, RecipeSingleModel, Recipe>
    {
        private readonly CursedContext db;
        public RecipeLogic(CursedContext db)
        {
            this.db = db;
            
        }

        public async Task<IEnumerable<RecipeAllModel>> GetAllDataModelAsync()
        {
            // merge into single query
            var recipes = await db.Recipe.ToListAsync();
            var a = from r in recipes
                    join ri in db.RecipeInheritance on r.Id equals ri.ChildId into t
                    group t by r.Id;
            var b = from r in recipes
                    join ri in db.RecipeInheritance on r.Id equals ri.ParentId into t
                    group t by r.Id;
            var c = from rr in (from r in recipes
                        join rpc in db.RecipeProductChanges on r.Id equals rpc.RecipeId into t
                        from t2 in t
                        join pc in db.ProductCatalog on t2.ProductId equals pc.Id into t3
                        from t4 in t3
                        select new
                        {
                            RecipeId = r.Id,
                            RecipeProductContainer = new RecipeProductContainer
                            {
                                ProductId = t2.ProductId,
                                CAS = t4.Cas,
                                ProductName = t4.Name,
                                Quantity = t2.Quantity,
                                Type = t2.Type
                            }
                        })
                    group rr by rr.RecipeId;

            var dd = from r in recipes
                     join ak in a on r.Id equals ak.Key into parentIdSingle
                     from pis in parentIdSingle.DefaultIfEmpty()
                     join bk in b on r.Id equals bk.Key into childIds
                     from ci in childIds.DefaultIfEmpty()
                     join ck in c on r.Id equals ck.Key into products
                     from p in products
                     select new RecipeAllModel
                     {
                         Id = r.Id,
                         Content = r.Content,
                         TechApproved = r.TechApproval,
                         GovApproved = r.GovermentApproval,
                         ParentRecipe = pis.Single().SingleOrDefault()?.ParentId,
                         ChildRecipesCount = ci.Single().Select(i => i.ChildId).Count(),
                         ProductCount = p.Select(i => i.RecipeProductContainer).Where(i=>i.Type==ProductCatalogTypes.Product).Count(),
                         MaterialCount = p.Select(i => i.RecipeProductContainer).Where(i => i.Type == ProductCatalogTypes.Material).Count()
                     };
            return dd.ToList();
            //Products = p.Select(i => i.RecipeProductContainer).ToList(),
        }
        public async Task<RecipeSingleModel> GetSingleDataModelAsync(object key)
        {
            throw new Exception("WorkInProgress");
        }
        public async Task<Recipe> GetSingleUpdateModelAsync(object key)
        {
            return await db.Recipe.SingleAsync(i => i.Id == (int)key);
        }
        public async Task AddDataModelAsync(Recipe dataModel)
        {
            dataModel.Id = default;
            db.Add(dataModel);
            await db.SaveChangesAsync();
        }
        public async Task UpdateDataModelAsync(Recipe updatedDataModel)
        {
            var currentModel = await db.Recipe.FirstOrDefaultAsync(i => i.Id == updatedDataModel.Id);
            db.Entry(currentModel).CurrentValues.SetValues(updatedDataModel);
            await db.SaveChangesAsync();
        }
        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.Recipe.FindAsync((int)key);

            db.Recipe.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}

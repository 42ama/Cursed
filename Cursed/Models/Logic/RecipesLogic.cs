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
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class RecipesLogic : IReadColection<RecipesModel>, IReadSingle<RecipeModel>, IReadUpdateForm<Recipe>, ICUD<Recipe>
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public RecipesLogic(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler<IEnumerable<RecipesModel>>> GetAllDataModelAsync()
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<RecipesModel>>("Recipes.");

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
                                             join pc in db.ProductCatalog on t2.ProductId equals pc.Id into t3
                                             from t4 in t3
                                             select new
                                             {
                                                 RecipeId = r.Id,
                                                 Type = t2.Type
                                             })
                                 group rr by rr.RecipeId) on r.Id equals ck.Key into products
                     from p in products
                     select new RecipesModel
                     {
                         Id = r.Id,
                         Content = r.Content,
                         TechApproved = r.TechApproval,
                         GovApproved = r.GovermentApproval,
                         ParentRecipe = pis.Single().SingleOrDefault()?.ParentId,
                         ChildRecipesCount = ci.Single().Select(i => i.ChildId).Count(),
                         ProductCount = p.Select(i => i.Type).Where(i=>i==ProductCatalogTypes.Product).Count(),
                         MaterialCount = p.Select(i => i.Type).Where(i=> i == ProductCatalogTypes.Material).Count()
                     };

            statusMessage.ReturnValue = query;

            return statusMessage;
        }
        public async Task<AbstractErrorHandler<RecipeModel>> GetSingleDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<RecipeModel>("Recipe.", key);

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
                        from p in products
                        select new RecipeModel
                        {
                            Id = r.Id,
                            Content = r.Content,
                            TechApproved = r.TechApproval,
                            GovApproved = r.GovermentApproval,
                            ParentRecipe = pis.Single().SingleOrDefault()?.ParentId,
                            ChildRecipes = ci.Single().Select(i => i.ChildId).ToList(),
                            RecipeProducts = p.Select(i => i.RecipeProductContainer).ToList()
                        };

            statusMessage.ReturnValue = query.Single();

            return statusMessage;
        }
        public async Task<AbstractErrorHandler<Recipe>> GetSingleUpdateModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<Recipe>("Recipe.", key);

            statusMessage.ReturnValue = await db.Recipe.SingleAsync(i => i.Id == (int)key);

            return statusMessage;
        }
        public async Task<AbstractErrorHandler> AddDataModelAsync(Recipe model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Recipe.");

            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }
        public async Task<AbstractErrorHandler> UpdateDataModelAsync(Recipe model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Recipe.", model.Id);

            var currentModel = await db.Recipe.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }
        public async Task<AbstractErrorHandler> RemoveDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Recipe.", key);

            var entity = await db.Recipe.FindAsync((int)key);
            db.Recipe.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

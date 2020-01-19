using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.StaticReferences;
using Cursed.Models.DataModel.Recipes;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.DataModel.RecipeProducts;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Recipes section logic. Consists of CRUD actions for recipes, including gathering methods for
    /// both single recipe and collection of all recipes.
    /// </summary>
    public class RecipesLogic : IReadColection<RecipesModel>, IReadSingle<RecipeModel>, IReadUpdateForm<Recipe>, ICUD<Recipe>
    {
        private readonly CursedDataContext db;
        public RecipesLogic(CursedDataContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather all recipes from database.
        /// </summary>
        /// <returns>All recipes from database. Each recipe contains more information than Recipe entity.</returns>
        public async Task<IEnumerable<RecipesModel>> GetAllDataModelAsync()
        {
            // gather recipes, to have unblocking call when grouping
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

        /// <summary>
        /// Gather single recipe, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Single recipe, which found by <c>key</c>. Contains more information than Recipe entity.</returns>
        public async Task<RecipeModel> GetSingleDataModelAsync(object key)
        {
            // gather recipes, to have unblocking call when grouping
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

        /// <summary>
        /// Gather single recipe, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Single recipe, which found by <c>key</c>.</returns>
        public async Task<Recipe> GetSingleUpdateModelAsync(object key)
        {
            return await db.Recipe.SingleAsync(i => i.Id == (int)key);
        }

        /// <summary>
        /// Add new recipe.
        /// </summary>
        /// <param name="model">Recipe to be added</param>
        /// <returns>Added recipe with correct key(Id) value</returns>
        public async Task<Recipe> AddDataModelAsync(Recipe model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Inverse TechApproval field of specific recipe
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
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

        /// <summary>
        /// Inverse GovermentApproval field of specific recipe
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
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

        /// <summary>
        /// Add new recipe, as child to existing recipe
        /// </summary>
        /// <param name="model">Recipe to be added</param>
        /// <param name="parentId">Id of recipe, which will be registered as parent</param>
        /// <returns>Added recipe with correct key(Id) value</returns>
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

        /// <summary>
        /// Update recipe.
        /// </summary>
        /// <param name="model">Updated recipe information</param>
        public async Task UpdateDataModelAsync(Recipe model)
        {
            var currentModel = await db.Recipe.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete recipe.
        /// </summary>
        /// <param name="key">Id of recipe to be deleted</param>
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

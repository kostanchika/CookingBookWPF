using CookingBookWPF.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace CookingBookWPF.Models
{
    internal static class DatabaseController
    {
        private static CookingBookContext _db;
        static DatabaseController()
        {
            try
            {
                _db = new();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw new Exception("капец");
            }
        }
        #region recipe
        public static void CreateRecipe(Recipe recipe)
        {
            try
            {
                TryAttach(recipe.Cooker);
                _db.Recipes.Add(recipe);
                _db.SaveChanges();
                GlobalResources.LoadIngredients();
                Messenger.Default.Send("refresh");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void DeleteRecipe(Recipe recipe)
        {
            try
            {
                var favorites = _db.Favorites.Where(f => f.Recipe.RecipeId == recipe.RecipeId);
                if (favorites != null)
                {
                    TryAttachRange(favorites);
                    _db.Favorites.RemoveRange(favorites);
                }
                TryAttach(recipe);
                _db.Recipes.Remove(recipe);
                _db.SaveChanges();
                GlobalResources.LoadIngredients();
                Messenger.Default.Send("refresh");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void UpdateRecipe(Recipe recipe)
        {
            try
            {
                TryAttach(recipe);
                //Recipe dbRecipe = _db.Recipes.First(r => r.RecipeId == recipe.RecipeId);
                //dbRecipe.Image = Recipe.Image;
                //dbRecipe.Name = Recipe.Name;
                //dbRecipe.CookingTime = Recipe.CookingTime;
                //dbRecipe.Cuisine = Recipe.Cuisine;
                //dbRecipe.Description = Recipe.Description;
                _db.Recipes.Update(recipe);
                _db.SaveChanges();
                GlobalResources.LoadIngredients();
                Messenger.Default.Send("refresh");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static Recipe? GetRecipe(Recipe recipe)
        {
            return _db.Recipes.Include(r => r.Ingredients).Include(r => r.Cooker).Include(r => r.Steps).FirstOrDefault(r => r.RecipeId == recipe.RecipeId);
        }
        #endregion
        #region favorite
        public static Favorite? GetFavorite(Recipe recipe)
        {
            return _db.Favorites.FirstOrDefault(f => f.User.UserId == GlobalResources.User.UserId && f.Recipe.RecipeId == recipe.RecipeId);
        }
        public static bool IsThereFavorite(Recipe recipe)
        {
            return GlobalResources.User == null || _db.Favorites.Any(f => f.User.UserId == GlobalResources.User.UserId && f.Recipe.RecipeId == recipe.RecipeId);
        }
        public static void CreateFavorite(Recipe recipe)
        {
            try
            {
                Favorite favorite = new(GlobalResources.User, recipe);
                TryAttach(GlobalResources.User);
                TryAttach(recipe);
                _db.Favorites.Add(favorite);
                _db.SaveChanges();
                Messenger.Default.Send("refresh");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void DeleteFavorite(Favorite favorite)
        {
            try
            {
                TryAttach(favorite);
                _db.Favorites.Remove(favorite);
                _db.SaveChanges();
                Messenger.Default.Send("refresh");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static int GetLikesCount(Recipe recipe)
        {
                return recipe == null ? 0 : _db.Favorites.Where(f => f.Recipe.RecipeId == recipe.RecipeId).Count();
        }
        #endregion
        #region subscription
        public static Subscription? GetSubscription(CookerProfile cookerProfile)
        {
            return _db.Subscriptions.FirstOrDefault(s => s.User.UserId == GlobalResources.User.UserId && s.CookerProfile.CookerProfileId == cookerProfile.CookerProfileId);
        }
        public static void CreateSubscription(CookerProfile cookerProfile)
        {
            Subscription subscription = new(GlobalResources.User, cookerProfile);
            TryAttach(subscription.CookerProfile);
            TryAttach(subscription.User);
            _db.Subscriptions.Add(subscription);
            _db.SaveChanges();

        }
        public static void DeleteSubscription(Subscription subscription)
        {
            TryAttach(subscription);
            _db.Subscriptions.Remove(subscription);
            _db.SaveChanges();
        }
        public static int GetSubscriptionsCount(CookerProfile cookerProfile)
        {
            return cookerProfile == null ? 0 : _db.Subscriptions.Where(s => s.CookerProfile.CookerProfileId == cookerProfile.CookerProfileId).Count();
        }
        public static bool IsThereSubscription(CookerProfile cookerProfile)
        {
            return cookerProfile == null || _db.Subscriptions.Any(s => s.User.UserId == GlobalResources.User.UserId && s.CookerProfile.CookerProfileId == cookerProfile.CookerProfileId);
        }
        #endregion
        #region cooker
        public static CookerProfile GetCookerProfile(int cookerProfileId)
        {
            return _db.CookerProfiles.AsNoTracking().First(c => c.CookerProfileId == cookerProfileId);
        }
        public static void UpdateCookerProfile(CookerProfile cookerProfile)
        {
            try
            {
                TryAttach(cookerProfile);
                _db.CookerProfiles.Update(cookerProfile);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static IEnumerable<Recipe> GetCookersRecipes(CookerProfile cookerProfile)
        {
            return _db.Recipes.AsNoTracking().Include(r => r.Cooker).Where(r => r.Cooker.CookerProfileId == cookerProfile.CookerProfileId);
        }
        #endregion
        #region user
        public static void AddUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public static bool IsThereLogin(string login)
        {
            return _db.Users.Any(u => u.Login == login);
        }

        public static User? GetUser(string login, string password)
        {
            return _db.Users.Where(u => u.Login == login && u.Password == password).Include(u => u.CookerProfile).FirstOrDefault();
        }
        #endregion

        public static IEnumerable<Recipe> LoadCatalog()
        {
            return _db.Recipes.AsNoTracking().Include(r => r.Cooker).Include(r => r.Ingredients);
        }

        public static IEnumerable<Recipe> LoadSubscriptions()
        {
            int[] subscriptions = [.. _db.Subscriptions
                        .Where(s => s.User.UserId == GlobalResources.User.UserId)
                        .Select(s => s.CookerProfile.CookerProfileId)];
            return _db.Recipes.AsNoTracking().Include(r => r.Cooker).Include(r => r.Ingredients)
                        .Where(r => subscriptions.Contains(r.Cooker.CookerProfileId));
        }

        public static IEnumerable<Recipe> LoadFavorites()
        {
            return _db.Favorites.AsNoTracking()
                        .Where(f => f.User.UserId == GlobalResources.User.UserId)
                        .Include(f => f.Recipe.Ingredients)
                        .Include(f => f.Recipe.Cooker).Select(f => f.Recipe);
        }

        public static IEnumerable<Ingredient> LoadIngredients()
        {
            return null; //_db.Recipes.AsNoTracking().Select(r => r.Ingredients);
        }

        private static void TryAttach(object obj)
        {
            try
            {
                _db.Attach(obj);
            }
            catch { }
        }

        private static void TryAttachRange(params object[] parameters)
        {
            foreach (var parameter in parameters)
            {
                TryAttach(parameter);
            }
        }
    }
}

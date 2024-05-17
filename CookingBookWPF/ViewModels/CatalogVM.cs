using CookingBookWPF.Commands;
using CookingBookWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CookingBookWPF.ViewModels
{
    enum Mode : byte
    {
        Catalog,
        Subscriptions,
        Favorites
    }
    internal class CatalogVM : ViewModelBase
    {
        private Mode _currentMode = Mode.Catalog;
        private FilterValues _filterValues = new("Все", "Все", "Все", string.Empty, "Все", string.Empty, "По умолчанию");
        public string CurrentMode { get => _currentMode == Mode.Catalog ? "Каталог" : _currentMode == Mode.Subscriptions ? "Подписки" : "Избранное"; }

        public ObservableCollection<Recipe> Recipes { get; set; }

        private RelayCommand<string> _setMode = null!;
        public RelayCommand<string> SetMode
        {
            get
            {
                return _setMode ??= new RelayCommand<string>(
                    (mode) =>
                    {
                        switch (mode)
                        {
                            case "Каталог":
                                _currentMode = Mode.Catalog;
                                break;
                            case "Подписки":
                                _currentMode = Mode.Subscriptions;
                                break;
                            case "Избранное":
                                _currentMode = Mode.Favorites;
                                break;
                        }
                        Load();
                    });
            }
        }

        private RelayCommand<Recipe> _selectRecipe = null!;
        public RelayCommand<Recipe> SelectRecipe
        {
            get
            {
                return _selectRecipe ??= new RelayCommand<Recipe>(
                    (recipe) =>
                    {
                        Messenger.Default.Send(recipe, "show");
                        Messenger.Default.Send(Views.Recipe);
                    });
            }
        }

        public CatalogVM()
        {
            Recipes = [.. DatabaseController.LoadCatalog()];
            Messenger.Default.Register<string>(this, (str) => { Load(); });
            Messenger.Default.Register<FilterValues>(this, "filter", filterValues => { this._filterValues = filterValues; Load(); });
        }
        private void Load()
        {
            RaisePropertyChanged(nameof(CurrentMode));
            switch (_currentMode)
            {
                case Mode.Catalog:
                    Recipes = [.. DatabaseController.LoadCatalog()];
                    break;
                case Mode.Subscriptions:
                    Recipes = [.. DatabaseController.LoadSubscriptions()];
                    break;
                case Mode.Favorites:
                    Recipes = [.. DatabaseController.LoadFavorites()];
                    break;
            }
            DoFilter(_filterValues);
        }

        private void DoFilter(FilterValues filterValues)
        {
            if (filterValues.Ingredients != string.Empty)
            {
                List<Recipe> recipes = new();
                if (filterValues.Ingredients != string.Empty)
                {
                    var ingredientList = filterValues.Ingredients.Split('\n');
                    ingredientList = ingredientList.Select(i => i.Trim()).ToArray();
                    foreach (var recipe in Recipes)
                    {
                        bool isGood = true;
                        foreach (var ingredient in recipe.Ingredients)
                        {
                            if (!ingredientList.Any(i => ingredient.Name.Contains(i, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                isGood = false;
                            }
                        }
                        if (isGood)
                        {
                            recipes.Add(recipe);
                        }
                    }
                }
                Recipes = [.. recipes.Where(r =>
                          (filterValues.Cuisine == "Все" || r.Cuisine == filterValues.Cuisine) &&
                          (filterValues.CookingTime == "Все" || r.CookingTime == filterValues.CookingTime) &&
                          (filterValues.Category == "Все" || r.Category == filterValues.Category) &&
                          (filterValues.Difficulty == "Все" || r.Difficulty == filterValues.Difficulty) &&
                          (string.IsNullOrEmpty(filterValues.Name) ||
                          r.Name.Contains(filterValues.Name, StringComparison.CurrentCultureIgnoreCase)))];
            } 
            else
            {
                Recipes = [.. Recipes.Where(r =>
                          (filterValues.Cuisine == "Все" || r.Cuisine == filterValues.Cuisine) &&
                          (filterValues.CookingTime == "Все" || r.CookingTime == filterValues.CookingTime) &&
                          (filterValues.Category == "Все" || r.Category == filterValues.Category) &&
                          (filterValues.Difficulty == "Все" || r.Difficulty == filterValues.Difficulty) &&
                          (string.IsNullOrEmpty(filterValues.Name) ||
                          r.Name.Contains(filterValues.Name, StringComparison.CurrentCultureIgnoreCase)))];
            }
            if (filterValues.Sort == "По названию")
                Recipes = [.. Recipes.OrderBy(r => r.Name)];
            if (filterValues.Sort == "По избранным")
                Recipes = [.. Recipes.OrderByDescending(DatabaseController.GetLikesCount)];
            if (filterValues.Sort == "По калорийности")
                Recipes = [.. Recipes.OrderByDescending(r => r.Calories)];
            if (filterValues.Sort == "По времени")
                Recipes = [.. Recipes.OrderBy(r => Array.IndexOf(GlobalResources.CookingTimes, r.CookingTime))];
        }
    }
}

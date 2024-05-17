using CookingBookWPF.Commands;
using CookingBookWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Text.RegularExpressions;

namespace CookingBookWPF.ViewModels
{
    internal class FiltersVM : ViewModelBase
    {
        public static string[] Cuisines { get => GlobalResources.Cuisines; }
        public string SelectedCuisine { get; set; } = "Все";
        public static string[] CookingTimes { get => GlobalResources.CookingTimes; }
        public string SelectedCookingTime { get; set; } = "Все";
        public static string[] Categories { get => GlobalResources.Categories; }
        public string SelectedCategory { get; set; } = "Все";
        public static string[] Difficulties { get => GlobalResources.Difficulties; }
        public string SelectedDifficulty { get; set; } = "Все";
        public static string[] Sortings { get => GlobalResources.Sortings; }
        public string SelectedSort { get; set; } = "По умолчанию";

        public string Name { get; set; } = string.Empty;


        private string _ingredients = string.Empty;
        public string Ingredients
        {
            get => _ingredients;
            set
            {
                if (!Regex.IsMatch(value, @"[\dA-Za-z$./;,+=@!#%^&*()_№:?]"))
                    _ingredients = value.TrimStart().Replace("  ", " ");
            }
        }

        private RelayCommand _applyFilters = null!;
        public RelayCommand ApplyFilters
        {
            get
            {
                return _applyFilters ??= new RelayCommand(
                    () =>
                    {
                        FilterValues filterValues = new(SelectedCuisine, SelectedCookingTime, SelectedDifficulty, Name, SelectedCategory, Ingredients, SelectedSort);
                        Messenger.Default.Send(filterValues, "filter");
                    });
            }
        }

        private RelayCommand _resetFilters = null!;
        public RelayCommand ResetFilters
        {
            get
            {
                return _resetFilters ??= new RelayCommand(
                    () =>
                    {
                        SelectedCuisine = "Все";
                        SelectedCookingTime = "Все";
                        SelectedCategory = "Все";
                        SelectedDifficulty = "Все";
                        SelectedSort = "По умолчанию";
                        Ingredients = string.Empty;
                        Name = string.Empty;
                        Messenger.Default.Send(new FilterValues(SelectedCuisine, SelectedCookingTime, SelectedDifficulty, Name, SelectedCategory, Ingredients, SelectedSort), "filter");
                    });
            }
        }
    }
}

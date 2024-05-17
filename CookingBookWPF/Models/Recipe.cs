using GalaSoft.MvvmLight;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace CookingBookWPF.Models
{
    [AddINotifyPropertyChangedInterface]
    internal class Recipe : ViewModelBase
    {
        public int RecipeId { get; set; }
        public byte[] Image { get; set; } = null!;

        private string _name = string.Empty;
        [MaxLength(50)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value.TrimStart().Replace("  ", " ");
            }
        }

        public int Calories { get; set; }
        public ObservableCollection<Ingredient> Ingredients { get; set; }
        public ObservableCollection<Step> Steps { get; set; }

        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Cuisine { get; set; } = string.Empty;
        public string CookingTime { get; set; } = string.Empty;
        public CookerProfile Cooker { get; set; } = null!;

        private Recipe()
        {
            Ingredients = [];
            Steps = [];
            Ingredients.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is Ingredient ingredient)
                        {
                            ingredient.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(IsCompleted));
                        }
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is Ingredient ingredient)
                        {
                            ingredient.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(IsCompleted));
                        }
                    }
                }
                RaisePropertyChanged(nameof(IsCompleted));
            };
            Steps.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is Step step)
                        {
                            step.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(IsCompleted));
                        }
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is Step step)
                        {
                            step.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(IsCompleted));
                        }
                    }
                }
                RaisePropertyChanged(nameof(IsCompleted));
            };

        }

        public Recipe(CookerProfile cooker) : this()
        {
            Cooker = cooker;
        }

        public bool IsCompleted
        {
            get => Image != null &&
                   Name != string.Empty &&
                   Calories != 0 &&
                   Difficulty != string.Empty &&
                   Cuisine != string.Empty &&
                   CookingTime != string.Empty &&
                   Ingredients != null &&
                   Ingredients.Count != 0 &&
                   Steps != null &&
                   Steps.Count != 0 &&
                   Steps.All(s => s.Image != null && s.Description != string.Empty) &&
                   Ingredients.All(i => i.Name != string.Empty && i.Quantity != 0 && i.QuantityType != string.Empty) &&
                   Category != string.Empty;
        }
    }
}

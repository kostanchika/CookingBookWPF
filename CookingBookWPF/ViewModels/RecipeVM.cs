using CookingBookWPF.Commands;
using CookingBookWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace CookingBookWPF.ViewModels
{
    internal class RecipeVM : ViewModelBase
    {
        public Recipe Recipe { get; set; }

        public bool IsOwner { get => GlobalResources.CookerProfile != null && Recipe.Cooker.CookerProfileId == GlobalResources.CookerProfile.CookerProfileId; }
        public bool IsDeleting { get; set; } = false;
        public static IEnumerable<string> Cuisines { get => GlobalResources.Cuisines.Skip(1); }
        public static IEnumerable<string> CookingTimes { get => GlobalResources.CookingTimes.Skip(1); }
        public static IEnumerable<string> Difficulties { get => GlobalResources.Difficulties.Skip(1); }
        public static IEnumerable<string> Categories { get => GlobalResources.Categories.Skip(1); }
        public static IEnumerable<string> QuantityTypes { get => GlobalResources.QuantityTypes; }
        public string LikesCount
        {
            get
            {
                int num = DatabaseController.GetLikesCount(Recipe);
                return "В избранном у " + num + " человек";
            }
        }

        private bool _isChanging;
        public bool IsChanging
        {
            get => _isChanging;
            set
            {
                _isChanging = value;
                RaisePropertyChanged(nameof(IsChanging));
                RaisePropertyChanged(nameof(IsOwner));
            }
        }

        private bool _isCreating;

        private RelayCommand _choosePhoto = null!; //Выбрать фото
        public RelayCommand ChoosePhoto
        {
            get
            {
                return _choosePhoto ??= new RelayCommand(
                    () =>
                    {
                        OpenFileDialog opf = new()
                        {
                            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
                        };
                        if (opf.ShowDialog() == true)
                        {
                            if (!opf.FileName.EndsWith(".png") &&
                                !opf.FileName.EndsWith(".jpeg") &&
                                !opf.FileName.EndsWith(".jpg"))
                                return;
                            Recipe.Image = File.ReadAllBytes(opf.FileName);
                            RaisePropertyChanged(nameof(Recipe));
                        }
                    });
            }
        }

        private RelayCommand _openChangeRecipe = null!; //Изменить/сохранить рецепт
        public RelayCommand OpenChangeRecipe
        {
            get
            {
                return _openChangeRecipe ??= new RelayCommand(
                    () =>
                    {
                        IsChanging = !IsChanging;
                        RaisePropertyChanged(nameof(EditButton));
                        Messenger.Default.Send(!IsChanging, "recipe");
                        if (!IsChanging)
                        {
                            if (_isCreating)
                            {
                                DatabaseController.CreateRecipe(Recipe);
                                _isCreating = false;
                            }
                            else
                            {
                                DatabaseController.UpdateRecipe(Recipe);
                            }
                        }
                    },
                    () => !IsDeleting);
            }
        }

        private RelayCommand _addToFavorites = null!; //Добавить в/убрать из избранное
        public RelayCommand AddToFavorites
        {
            get
            {
                return _addToFavorites ??= new RelayCommand(
                    () =>
                    {
                        Favorite? favorite = DatabaseController.GetFavorite(Recipe);
                        if (favorite != null)
                        {
                            DatabaseController.DeleteFavorite(favorite);
                        }
                        else
                        {
                            DatabaseController.CreateFavorite(Recipe);
                        }
                        RaisePropertyChanged(nameof(FavButton));
                        RaisePropertyChanged(nameof(LikesCount));
                    },
                    () => !IsDeleting);
            }
        }

        private RelayCommand _openCooker = null!; //Открыть профиль повара
        public RelayCommand OpenCooker
        {
            get
            {
                return _openCooker ??= new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(Recipe.Cooker, "show");
                        Messenger.Default.Send(Views.Profile);
                    },
                    () => !IsDeleting);
            }
        }

        private RelayCommand _deleteRecipe = null!; //Удалить рецепт
        public RelayCommand DeleteRecipe
        {
            get
            {
                return _deleteRecipe ??= new RelayCommand(
                    () =>
                    {
                        IsDeleting = true;
                        Messenger.Default.Send(false, "recipe");
                    },
                    () => !IsDeleting);
            }
        }

        private RelayCommand _deleteRecipeNo = null!; //Удалить рецепт
        public RelayCommand DeleteRecipeNo
        {
            get
            {
                return _deleteRecipeNo ??= new RelayCommand(
                    () =>
                    {
                        IsDeleting = false;
                        Messenger.Default.Send(true, "recipe");
                    });
            }
        }

        private RelayCommand _deleteRecipeYes = null!; //Удалить рецепт
        public RelayCommand DeleteRecipeYes
        {
            get
            {
                return _deleteRecipeYes ??= new RelayCommand(
                    () =>
                    {
                        DatabaseController.DeleteRecipe(Recipe);
                        Messenger.Default.Send(Views.Catalog);
                        IsDeleting = false;
                        Messenger.Default.Send(true, "recipe");
                    });
            }
        }

        private RelayCommand<Step> _selectStepPhoto = null!;
        public RelayCommand<Step> SelectStepPhoto
        {
            get
            {
                return _selectStepPhoto ??= new RelayCommand<Step>(
                    (step) =>
                    {
                        OpenFileDialog opf = new()
                        {
                            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
                        };
                        if (opf.ShowDialog() == true)
                        {
                            if (!opf.FileName.EndsWith(".png") &&
                                !opf.FileName.EndsWith(".jpeg") &&
                                !opf.FileName.EndsWith(".jpg"))
                            {
                                return;
                            }
                            step.Image = File.ReadAllBytes(opf.FileName);
                            RaisePropertyChanged(nameof(Recipe));
                        }
                    });
            }
        }

        private RelayCommand<Step> _upStep = null!;
        public RelayCommand<Step> UpStep
        {
            get
            {
                return _upStep ??= new RelayCommand<Step>(
                    (step) =>
                    {
                        int index = Recipe.Steps.IndexOf(step);
                        if (index != 0)
                        {
                            (Recipe.Steps[index - 1], Recipe.Steps[index]) = (Recipe.Steps[index], Recipe.Steps[index - 1]);
                        }
                    });
            }
        }

        private RelayCommand<Step> _downStep = null!;
        public RelayCommand<Step> DownStep
        {
            get
            {
                return _downStep ??= new RelayCommand<Step>(
                    (step) =>
                    {
                        int index = Recipe.Steps.IndexOf(step);
                        if (index != Recipe.Steps.Count - 1)
                        {
                            (Recipe.Steps[index + 1], Recipe.Steps[index]) = (Recipe.Steps[index], Recipe.Steps[index + 1]);
                        }
                    });
            }
        }

        private RelayCommand<Step> _deleteStep = null!;
        public RelayCommand<Step> DeleteStep
        {
            get
            {
                return _deleteStep ??= new RelayCommand<Step>(
                    (step) =>
                    {
                        Recipe.Steps.Remove(step);
                    });
            }
        }

        private RelayCommand _addStep = null!;
        public RelayCommand AddStep
        {
            get
            {
                return _addStep ??= new RelayCommand(
                    () =>
                    {
                        Recipe.Steps.Add(new Step());
                    });
            }
        }

        private RelayCommand<Ingredient> _deleteIngredient = null!;
        public RelayCommand<Ingredient> DeleteIngredient
        {
            get
            {
                return _deleteIngredient ??= new RelayCommand<Ingredient>(
                    (ingredient) =>
                    {
                        Recipe.Ingredients.Remove(ingredient);
                    });
            }
        }

        private RelayCommand _addIngredient = null!;
        public RelayCommand AddIngredient
        {
            get
            {
                return _addIngredient ??= new RelayCommand(
                    () =>
                    {
                        Recipe.Ingredients.Add(new Ingredient());
                    });
            }
        }

        public string FavButton { get => DatabaseController.IsThereFavorite(Recipe) ? "убрать из избранного" : "в избранное"; }
        public string EditButton { get => IsChanging ? "сохранить" : "изменить"; }

        public RecipeVM()
        {
            Messenger.Default.Register<Recipe>(this, "show", async (recipe) => { await ChangeRecipe(recipe); });
            Messenger.Default.Register<Recipe>(this, "create", CreateRecipe);
        }

        private async Task ChangeRecipe(Recipe recipe)
        {
            _isCreating = false;
            IsChanging = false;
            IsDeleting = false;
            Recipe = DatabaseController.GetRecipe(recipe);
            RaisePropertyChanged(nameof(FavButton));
            RaisePropertyChanged(nameof(IsOwner));
            RaisePropertyChanged(nameof(LikesCount));
        }

        private void CreateRecipe(Recipe recipe)
        {
            Recipe = recipe;
            IsChanging = true;
            _isCreating = true;
        }
    }
}

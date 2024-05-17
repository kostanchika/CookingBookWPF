using CookingBookWPF.Commands;
using CookingBookWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace CookingBookWPF.ViewModels
{
    internal class ProfileVM : ViewModelBase
    {
        public bool IsOwner { get; set; } = true;
        public bool IsChanging { get; set; }
        public CookerProfile CookerProfile { get; set; } = GlobalResources.CookerProfile;
        public ObservableCollection<Recipe> Recipes { get; set; } = null!;

        private RelayCommand _openChangeProfile = null!; //Изменить/сохранить профиль
        public RelayCommand OpenChangeProfile
        {
            get
            {
                return _openChangeProfile ??= new RelayCommand(
                    () =>
                    {
                        IsChanging = !IsChanging;
                        RaisePropertyChanged(nameof(EditButton));
                        if (IsChanging)
                        {
                            Messenger.Default.Send(false, "recipe");
                        }
                        else
                        if (IsChanging == false)
                        {
                            Messenger.Default.Send(true, "recipe");
                            DatabaseController.UpdateCookerProfile(CookerProfile);
                            GlobalResources.User.CookerProfile = CookerProfile;
                            Load(CookerProfile);
                            Messenger.Default.Send("refresh");
                        }
                    },
                    () =>
                    {
                        return CookerProfile == null ||
                               CookerProfile.Avatar != null &&
                               CookerProfile.ProfileName != string.Empty &&
                               CookerProfile.Description != string.Empty;
                    });
            }
        }

        private RelayCommand _createSubscription = null!; //Подписаться/отписаться
        public RelayCommand CreateSubscription
        {
            get
            {
                return _createSubscription ??= new RelayCommand(
                    () =>
                    {
                        Subscription? subscription = DatabaseController.GetSubscription(CookerProfile);
                        if (subscription != null)
                            DatabaseController.DeleteSubscription(subscription);
                        else
                            DatabaseController.CreateSubscription(CookerProfile);
                        Messenger.Default.Send(subscription);
                        RaisePropertyChanged(nameof(SubButton));
                        RaisePropertyChanged(nameof(SubCount));
                    });
            }
        }

        private RelayCommand _choosePhoto = null!; //Выбрать фото
        public RelayCommand ChoosePhoto
        {
            get
            {
                return _choosePhoto ??= new RelayCommand(
                    () =>
                    {
                        if (!IsChanging)
                            return;
                        OpenFileDialog opf = new()
                        {
                            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
                        };
                        if (opf.ShowDialog() == true)
                        {
                            if (!opf.FileName.EndsWith(".png") &&
                                !opf.FileName.EndsWith(".jpeg") &&
                                !opf.FileName.EndsWith(".jpg"))
                                return;
                            CookerProfile.Avatar = File.ReadAllBytes(opf.FileName);
                            RaisePropertyChanged(nameof(CookerProfile));
                        }
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

        public string SubCount
        {
            get
            {
                int num = DatabaseController.GetSubscriptionsCount(CookerProfile);
                return num + " подписчик" + GetEndWordOv(num);
            }
        }
        public string SubButton { get => DatabaseController.IsThereSubscription(CookerProfile) ? "отписаться" : "подписаться"; }
        public string EditButton { get => IsChanging ? "сохранить" : "изменить"; }

        public ProfileVM()
        {
            Messenger.Default.Register<CookerProfile>(this, "show", ChangeCooker);
        }
        private void ChangeCooker(CookerProfile cooker)
        {
            Load(cooker);
            CookerProfile = DatabaseController.GetCookerProfile(cooker.CookerProfileId);
            RaisePropertyChanged(nameof(SubCount));
            if (GlobalResources.CookerProfile == null || cooker.CookerProfileId != GlobalResources.CookerProfile.CookerProfileId)
            {
                IsOwner = false;
                RaisePropertyChanged(nameof(SubButton));
                return;
            }
            IsOwner = true;
        }

        private void Load(CookerProfile cooker) => Recipes = [.. DatabaseController.GetCookersRecipes(cooker)];

        public static string GetEndWordOv(int integer)
        {
            int lastInt = integer % 10;

            if ((integer >= 5 && integer <= 20) ||
                (integer == 0) ||
                (integer >= 20 && lastInt >= 5 && lastInt <= 9) ||
                (integer >= 20 && lastInt == 0))
                return "ов";
            else if ((integer >= 2 && integer <= 4) ||
                     (lastInt >= 2 && lastInt <= 4))
                return "а";

            return string.Empty;
        }
    }
}
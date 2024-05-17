using CookingBookWPF.Commands;
using CookingBookWPF.Models;
using CookingBookWPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PropertyChanged;
using System.Windows;
using System.Windows.Controls;

namespace CookingBookWPF.ViewModels
{
    enum Views : byte
    {
        Authorization,
        Registration,
        Catalog,
        Recipe,
        Profile,
    }
    [AddINotifyPropertyChangedInterface]
    internal class ViewsVM : ViewModelBase
    {
        private bool _canChangeView = true;
        public bool CanChangeView
        {
            get => _canChangeView;
            set
            {
                _canChangeView = value;
                RaisePropertyChanged(nameof(IsCooker));
                RaisePropertyChanged(nameof(CanChangeView));
            }
        }
        public bool IsCooker { get; set; }

        private readonly UserControl[] _views = [
                                                    new Authorization(),
                                                    new Registration(),
                                                    new Catalog(),
                                                    new RecipePage(),
                                                    new Profile(),
                                                ];

        private UserControl _currentView = null!;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                if (value != _currentView)
                {
                    if (CanChangeView)
                        _currentView = value;
                    RaisePropertyChanged(nameof(CurrentView));
                }
            }
        }

        private RelayCommand _changeViewBurgerRecipes = null!;
        public RelayCommand ChangeViewBurgerRecipes
        {
            get
            {
                return _changeViewBurgerRecipes ??= new RelayCommand(
                    () =>
                    {
                        CurrentView = _views[(byte)Views.Catalog];
                    });
            }
        }

        private RelayCommand _changeViewBurgerProfile = null!;
        public RelayCommand ChangeViewBurgerProfile
        {
            get
            {
                return _changeViewBurgerProfile ??= new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(GlobalResources.CookerProfile, "show");
                        CurrentView = _views[(byte)Views.Profile];
                    });
            }
        }

        private RelayCommand _changeViewBurgerCreateRecipe = null!;
        public RelayCommand ChangeViewBurgerCreateRecipe
        {
            get
            {
                return _changeViewBurgerCreateRecipe ??= new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(new Recipe(GlobalResources.CookerProfile), "create");
                        CurrentView = _views[(byte)Views.Recipe];
                    });
            }
        }

        private RelayCommand _logout = null!;
        public RelayCommand Logout
        {
            get
            {
                return _logout ??= new RelayCommand(
                    () =>
                    {
                        if (CanChangeView)
                        {
                            CurrentView = _views[(byte)Views.Authorization];
                            CanChangeView = false;
                            IsCooker = false;
                        }
                    });
            }
        }

        private RelayCommand<Window> _closeWindow = null!; //Закрытие окна
        public RelayCommand<Window> CloseWindow
        {
            get
            {
                return _closeWindow ??= new RelayCommand<Window>(
                    (window) =>
                    {
                        window.Close();
                    });
            }
        }

        private RelayCommand<Window> _minimizeWindow = null!;
        public RelayCommand<Window> MinimizeWindow
        {
            get
            {
                return _minimizeWindow ??= new RelayCommand<Window>(
                    (window) =>
                    {
                        window.WindowState = WindowState.Minimized;
                    });
            }
        }

        public ViewsVM()
        {
            _currentView = _views[(byte)Views.Authorization];
            CanChangeView = false;
             Messenger.Default.Register<Views>(this, (view) =>
            {
                if (view == Views.Registration || view == Views.Authorization)
                {
                    CanChangeView = true;
                    CurrentView = _views[(byte)view];
                    CanChangeView = false;
                }
                else
                    CurrentView = _views[(byte)view];
            });
            Messenger.Default.Register<bool>(this, "recipe", (value) => CanChangeView = value);
            Messenger.Default.Register<bool>(this, "cooker", (value) => IsCooker = value);
        }
    }
}

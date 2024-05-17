using CookingBookWPF.Commands;
using CookingBookWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace CookingBookWPF.ViewModels
{
    enum ValidateMode : byte
    {
        Register,
        Login,
    }
    internal partial class AuthorizationVM : ViewModelBase
    {
        public User RegisteringUser { get; set; } = new();
        public User LoginingUser { get; set; } = new();
        public string LoginError { get; set; } = string.Empty;
        public string PasswordError { get; set; } = string.Empty;
        public bool IsCooker { get; set; } = false;

        private RelayCommand _backWindow = null!; //Вернутся к главному окну
        public RelayCommand BackWindow
        {
            get
            {
                return _backWindow ??= new RelayCommand(
                    () =>
                    {
                        LoginError = string.Empty;
                        PasswordError = string.Empty;
                        Messenger.Default.Send(Views.Authorization);
                    });
            }
        }

        private RelayCommand _openRegistration = null!; //Открыть окно регистрации
        public RelayCommand OpenRegistration
        {
            get
            {
                return _openRegistration ??= new RelayCommand(
                    () =>
                    {
                        IsCooker = false;
                        LoginError = string.Empty;
                        PasswordError = string.Empty;
                        RegisteringUser = new()
                        {
                            CookerProfile = new()
                        };
                        Messenger.Default.Send(Views.Registration);
                    });
            }
        }

        private RelayCommand _createAccount = null!; //Зарегистрировать аккаунт
        public RelayCommand CreateAccount
        {
            get
            {
                return _createAccount ??= new RelayCommand(
                    () =>
                    {
                        if (!ValidateInput(ValidateMode.Register))
                            return;
                        if (!IsCooker)
                            RegisteringUser.CookerProfile = null;
                        DatabaseController.AddUser(RegisteringUser);
                        Messenger.Default.Send(Views.Authorization);
                    },
                    () =>
                    {
                        if (IsCooker && RegisteringUser.CookerProfile != null)
                        {
                            return RegisteringUser.CookerProfile.Avatar != null &&
                                   RegisteringUser.CookerProfile.ProfileName != string.Empty &&
                                   RegisteringUser.CookerProfile.Description != string.Empty;
                        }
                        return true;
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
                            RegisteringUser.CookerProfile.Avatar = File.ReadAllBytes(opf.FileName);
                            RaisePropertyChanged(nameof(RegisteringUser));
                        }
                    });
            }
        }

        private RelayCommand _login = null!; //Вход в приложение
        public RelayCommand Login
        {
            get
            {
                return _login ??= new RelayCommand(
                    () =>
                    {
                        if (!ValidateInput(ValidateMode.Login))
                            return;
                        GlobalResources.User = LoginingUser;
                        GlobalResources.CookerProfile = LoginingUser.CookerProfile;
                        Messenger.Default.Send(LoginingUser.CookerProfile != null, "cooker");
                        Messenger.Default.Send(true, "recipe");
                        Messenger.Default.Send(Views.Catalog);
                        LoginingUser = new();
                    });
            }
        }

        private bool ValidateInput(ValidateMode validateMode)
        {
            LoginError = string.Empty;
            PasswordError = string.Empty;
            switch (validateMode)
            {
                case ValidateMode.Register:
                    if (RegisteringUser.Password.Length < 6)
                        PasswordError = "Поле \"Пароль\" должно содержать минимум 6 символов";
                    if (!AuthorizationRegex().IsMatch(RegisteringUser.Login))
                        LoginError = "Поле \"Логин\" может содержать только символы латинского алфавита, а также цифры";
                    if (!AuthorizationRegex().IsMatch(RegisteringUser.Password))
                        PasswordError = "Поле \"Пароль\" может содержать только символы латинского алфавита, а также цифры";

                    if (string.IsNullOrWhiteSpace(RegisteringUser.Login))
                        LoginError = "Поле \"Логин\" должно быть заполнено";
                    if (string.IsNullOrWhiteSpace(RegisteringUser.Password))
                        PasswordError = "Поле \"Пароль\" должно быть заполнено";
                    else if (DatabaseController.IsThereLogin(RegisteringUser.Login))
                        LoginError = "Данный логин уже занят, придумайте другой";
                    break;
                case ValidateMode.Login:
                    if (string.IsNullOrEmpty(LoginingUser.Login))
                        LoginError = "Поле \"Логин\" должно быть заполнено";
                    if (string.IsNullOrEmpty(LoginingUser.Password))
                        PasswordError = "Поле \"Пароль\" должно быть заполнено";
                    if (LoginError == string.Empty && PasswordError == string.Empty)
                    {
                        User? bufferUser = DatabaseController.GetUser(LoginingUser.Login, LoginingUser.Password);
                        if (bufferUser == null)
                            LoginError = "Пользователь с данным логином не найден/неверный пароль";
                        else
                            LoginingUser = bufferUser;
                    }
                    break;
            }
            if (LoginError == string.Empty && PasswordError == string.Empty)
                return true;
            return false;
        }

        [GeneratedRegex(@"^[A-Za-z0-9]+$")]
        private static partial Regex AuthorizationRegex();
    }
}

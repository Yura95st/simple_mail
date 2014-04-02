using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System.Windows.Input;
using System;
using data_models.Exceptions;
using data_models.Validations;

namespace simple_mail.ViewModels
{
    public class SettingsViewModel : BaseViewModel, IPageViewModel
    {
        private UserDbHelper _userDbHelper = UserDbHelper.Instance;
        private string _oldPassword;
        private string _newPassword;
        private string _confirmNewPassword;

        private ICommand _changePasswordCommand;

        public SettingsViewModel()
        {
        }

        public string OldPassword
        {
            get
            {
                return _oldPassword;
            }
            set
            {
                if (value != _oldPassword)
                {
                    _oldPassword = value;
                    OnPropertyChanged("OldPassword");
                }
            }
        }
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                if (value != _newPassword)
                {
                    _newPassword = value;
                    OnPropertyChanged("NewPassword");
                }
            }
        }
        public string ConfirmNewPassword
        {
            get
            {
                return _confirmNewPassword;
            }
            set
            {
                if (value != _confirmNewPassword)
                {
                    _confirmNewPassword = value;
                    OnPropertyChanged("ConfirmNewPassword");
                }
            }
        }

        public ICommand ChangePasswordCommand
        {
            get
            {
                if (_changePasswordCommand == null)
                {
                    _changePasswordCommand = new RelayCommand(
                        param => ChangePassword(),
                        param => AreValidPasswordFields()
                    );
                }
                return _changePasswordCommand;
            }
        }

        private bool AreValidPasswordFields()
        {
            return !(string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword)
                || string.IsNullOrEmpty(ConfirmNewPassword));
        }

        private void ChangePassword()
        {
            if (!MyValidation.AreTwoPasswordsEqual(NewPassword, ConfirmNewPassword))
            {
                _notification.Text = "Passwords are not equal!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            User user = null;

            try
            {
                user = _userDbHelper.GetUserById(AuthorizationViewModel.LoggedUserId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            if (user == null)
            {
                _notification.Text = "An error occured. Please, try again later.";
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            if (!MyValidation.AreTwoPasswordsEqual(user.Password, MyValidation.Hash(OldPassword, user.Email)))
            {
                _notification.Text = "Old password is invalid!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            user.Password = NewPassword;

            try 
            {
                _userDbHelper.ChangePassword(user);
            }
            catch (InvalidPasswordException e)
            {
                _notification.Text = "Password must: be between 6 and 30 characters long, contain at least one number, uppercase letter, lowercase letter, special character.";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            _notification.Text = "Password was successfully changed.";
            _notification.Type = (int)Notification.Types.Info;
            ApplicationViewModel.ShowNotificationBox(_notification);
            return;
        }

        public void OnShow()
        {
            OldPassword = "";
            NewPassword = "";
            ConfirmNewPassword = "";
        }
    }
}

using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System.Windows.Input;
using System;
using data_models.Exceptions;

namespace simple_mail.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel, IPageViewModel
    {
        private User _userModel = new User();
        private UserDbHelper _userDbHelper = UserDbHelper.Instance;

        private ICommand _signInCommand;
        private ICommand _logOutCommand;

        public static int LoggedUserId = 0;

        public AuthorizationViewModel()
        {
            //UserModel.Email = "admin@gmail.com";
            //UserModel.Password = "admin";
        }

        public User UserModel
        {
            get
            {
                return _userModel;
            }
            set
            {
                if (value != _userModel)
                {
                    _userModel = value;
                    OnPropertyChanged("UserModel");
                }
            }
        }

        public ICommand SignInCommand
        {
            get
            {
                if (_signInCommand == null)
                {
                    _signInCommand = new RelayCommand(
                        param => AuthorizeUser(),
                        param => AreValidUserFields()
                    );
                }
                return _signInCommand;
            }
        }

        public ICommand LogOutCommand
        {
            get
            {
                if (_logOutCommand == null)
                {
                    _logOutCommand = new RelayCommand(
                        param => LogOutUser(),
                        param => IsUserLoggedIn()
                    );
                }
                return _logOutCommand;
            }
        }

        public void OnShow()
        {
            UserModel.Email = "";
            UserModel.Password = "";
        }

        private void AuthorizeUser()
        {
            int userId = 0;

            try
            {
                userId = _userDbHelper.LogInUser(UserModel);
            }
            catch (InvalidEmailException e)
            {
                _notification.Text = "Email is invalid!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch (UserDoesNotExistException e)
            {
                _notification.Text = "Couldn't log you in as " + UserModel.Email;
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch (InvalidPasswordException e)
            {
                _notification.Text = "Password is invalid!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            LoggedUserId = userId;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.UserLoggedIn);
        }

        private void LogOutUser()
        {
            try
            {
                _userDbHelper.LogOutUser(LoggedUserId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
            }

            LoggedUserId = 0;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.UserLoggedOut);
        }

        private bool AreValidUserFields()
        {
            return !(string.IsNullOrEmpty(UserModel.Email) || string.IsNullOrEmpty(UserModel.Password));
        }

        private bool IsUserLoggedIn()
        {
            return LoggedUserId > 0;
        }
    }
}

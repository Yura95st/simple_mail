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
        private string _signInInfoMsg = "";

        public static int LoggedUserId = 0;

        public AuthorizationViewModel()
        {
            UserModel.Email = "admin@gmail.com";
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

        public string SignInInfoMsg
        {
            get
            {
                return _signInInfoMsg;
            }
            set
            {
                if (value != _signInInfoMsg)
                {
                    _signInInfoMsg = value;
                    OnPropertyChanged("SignInInfoMsg");
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
            //UserModel.Email = "";
            UserModel.Password = "";
        }

        private void AuthorizeUser()
        {
            SignInInfoMsg = "";

            int userId = 0;

            try
            {
                userId = _userDbHelper.LogInUser(UserModel);
            }
            catch (InvalidEmailException e)
            {
                SignInInfoMsg = "INFO: Email is invalid!";
                return;
            }
            catch (UserDoesNotExistException e)
            {
                SignInInfoMsg = "INFO: Couldn't log you in as " + UserModel.Email;
                return;
            }
            catch (InvalidPasswordException e)
            {
                SignInInfoMsg = "INFO: Password is invalid!";
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
                return;
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

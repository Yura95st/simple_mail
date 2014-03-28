using data_models.Validations;
using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using simple_mail.Models;
using System.Windows.Input;
using System;
using data_models.Exceptions;

namespace simple_mail.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private UserModel _userModel = new UserModel();
        private ICommand _signInCommand;
        private ICommand _logOutCommand;
        private string _signInInfoMsg = "";

        public static int LoggedUserId = 0;

        public AuthorizationViewModel()
        {
            UserAccount.Email = "admin@gmail.com";
            UserAccount.Password = "admin";
        }

        public UserModel UserAccount
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
                    OnPropertyChanged("UserAccount");
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

        private void AuthorizeUser()
        {
            SignInInfoMsg = "";

            UserDbHelper userDbHelper = new UserDbHelper();

            int userId = 0;

            try
            {
                userId = userDbHelper.LogInUser(UserAccount.User);
            }
            catch (InvalidEmailException e)
            {
                SignInInfoMsg = "INFO: Email is invalid!";
                return;
            }
            catch (UserDoesNotExistException e)
            {
                SignInInfoMsg = "INFO: Couldn't log you in as " + UserAccount.Email;
                return;
            }
            catch (InvalidPasswordException e)
            {
                SignInInfoMsg = "INFO: Password is invalid!";
                return;
            }

            AuthorizationViewModel.LoggedUserId = userId;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.UserLoggedIn);
        }

        private void LogOutUser()
        {
            UserDbHelper userDbHelper = new UserDbHelper();

            try
            {
                userDbHelper.LogOutUser(AuthorizationViewModel.LoggedUserId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return;
            }

            AuthorizationViewModel.LoggedUserId = 0;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.UserLoggedOut);
        }

        private bool AreValidUserFields()
        {
            return !(string.IsNullOrEmpty(UserAccount.Email) || string.IsNullOrEmpty(UserAccount.Password));
        }

        private bool IsUserLoggedIn()
        {
            return AuthorizationViewModel.LoggedUserId > 0;
        }
    }
}

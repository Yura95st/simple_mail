using data_models.Validations;
using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using simple_mail.Models;
using System.Windows.Input;
using System;

namespace simple_mail.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private UserModel _userModel = new UserModel();
        private ICommand _signInCommand;
        private string _signInInfoMsg = "";

        public AuthorizationViewModel()
        { }

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

        private void AuthorizeUser()
        {
            SignInInfoMsg = "";

            if (!MyValidation.IsValidEmail(UserAccount.Email))
            {
                SignInInfoMsg = "INFO: Email is invalid!";
                return;
            }

            UserDbHelper userDbHelper = new UserDbHelper();
            User userInDb = null;
            
            try 
            {                
                userInDb = userDbHelper.GetUserByEmail(UserAccount.Email);
            }
            catch(Exception e)
            {}

            if (!String.Equals(userInDb.Password, MyValidation.Hash(UserAccount.Password, userInDb.Email)))
            {
                SignInInfoMsg = "INFO: Password is invalid!";
                return;
            }
        }

        private bool AreValidUserFields()
        {
            return !(string.IsNullOrEmpty(UserAccount.Email) || string.IsNullOrEmpty(UserAccount.Password));
        }
    }
}

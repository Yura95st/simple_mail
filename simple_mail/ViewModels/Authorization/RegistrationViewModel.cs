using data_models.Models;
using data_models.Validations;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using simple_mail.Models;
using System;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private UserModel _userModel = new UserModel();
        private ICommand _signUpCommand;
        private string _confirmPassword = "";
        private string _signUpInfoMsg = "";

        public RegistrationViewModel()
        {
            UserAccount.Email = "yura95st@gmail.com";
            UserAccount.FirstName = "Yura";
            UserAccount.Password = "1111";
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

        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                if (value != _confirmPassword)
                {
                    _confirmPassword = value;
                    OnPropertyChanged("ConfirmPassword");
                }
            }
        }

        public string SignUpInfoMsg
        {
            get
            {
                return _signUpInfoMsg;
            }
            set
            {
                if (value != _signUpInfoMsg)
                {
                    _signUpInfoMsg = value;
                    OnPropertyChanged("SignUpInfoMsg");
                }
            }
        }

        public ICommand SignUpCommand
        {
            get
            {
                if (_signUpCommand == null)
                {
                    _signUpCommand = new RelayCommand(
                        param => CreateNewUserAccount(),
                        param => AreValidUserFields()
                    );
                }
                return _signUpCommand;
            }
        }

        private void CreateNewUserAccount()
        {
            SignUpInfoMsg = "";

            if (!MyValidation.IsValidEmail(UserAccount.Email))
            {
                SignUpInfoMsg = "INFO: Email is invalid!";
                return;
            }

            if (!MyValidation.IsValidFirstName(UserAccount.FirstName))
            {
                SignUpInfoMsg = "INFO: UserName is invalid!";
                return;
            }

            if (!ArePasswordEqual())
            {
                SignUpInfoMsg = "INFO: Passwords are not equal!";
                return;
            }

            UserDbHelper userDbHelper = new UserDbHelper();

            try
            {
                int newUserId = userDbHelper.AddNewUser(UserAccount.User);
                //TODO: Set user online
            }
            catch (Exception e)
            {
                SignUpInfoMsg = "INFO: UserAccount with such email already exists!";
            }
        }

        private bool AreValidUserFields()
        {
            return !(string.IsNullOrEmpty(UserAccount.FirstName) || string.IsNullOrEmpty(UserAccount.Email) || string.IsNullOrEmpty(UserAccount.Password) 
                || string.IsNullOrEmpty(this.ConfirmPassword));
        }

        private bool ArePasswordEqual()
        {
            return (string.Equals(UserAccount.Password, ConfirmPassword));
        }
    }
}

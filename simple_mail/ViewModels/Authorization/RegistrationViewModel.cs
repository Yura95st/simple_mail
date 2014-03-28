using data_models.Exceptions;
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
            UserAccount.FirstName = "Admin";
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

            UserDbHelper userDbHelper = new UserDbHelper();

            if (!MyValidation.AreTwoPasswordsEqual(UserAccount.Password, ConfirmPassword))
            {
                SignUpInfoMsg = "INFO: Passwords are not equal!";
                return;
            }

            try
            {
                userDbHelper.AddNewUser(UserAccount.User);
            }
            catch (InvalidEmailException e)
            {
                SignUpInfoMsg = "INFO: Email is invalid!";
                return;
            }
            catch (InvalidFirstNameException e)
            {
                SignUpInfoMsg = "INFO: Username is invalid!";
                return;
            }
            catch (InvalidPasswordException e)
            {
                SignUpInfoMsg = "INFO: Password is invalid!";
                return;
            }
            catch (UserAlreadyExistsException e)
            {
                SignUpInfoMsg = "INFO: UserAccount with such email already exists!";
                return;
            }

            int userId = 0;

            try
            {
                userId = userDbHelper.LogInUser(UserAccount.User);
            }
            catch (Exception e)
            {
                SignUpInfoMsg = "INFO: An error occured while logging You in!";
                return;
            }

            AuthorizationViewModel.LoggedUserId = userId;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.UserLoggedIn);
        }

        private bool AreValidUserFields()
        {
            return !(string.IsNullOrEmpty(UserAccount.FirstName) || string.IsNullOrEmpty(UserAccount.Email) || string.IsNullOrEmpty(UserAccount.Password) 
                || string.IsNullOrEmpty(this.ConfirmPassword));
        }
    }
}

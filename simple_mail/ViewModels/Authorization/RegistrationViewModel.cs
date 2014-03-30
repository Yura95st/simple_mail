using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class RegistrationViewModel : BaseViewModel, IPageViewModel
    {
        private User _userModel = new User();
        private UserDbHelper _userDbHelper = UserDbHelper.Instance;

        private ICommand _signUpCommand;
        private string _confirmPassword = "";
        private string _signUpInfoMsg = "";

        public RegistrationViewModel()
        {
            //UserModel.FirstName = "Admin";
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

        public void OnShow()
        {
            UserModel.FirstName = "";
            UserModel.Email = "";
            UserModel.Password = "";
        }

        private void CreateNewUserAccount()
        {
            SignUpInfoMsg = "";

            if (!MyValidation.AreTwoPasswordsEqual(UserModel.Password, ConfirmPassword))
            {
                SignUpInfoMsg = "INFO: Passwords are not equal!";
                return;
            }

            try
            {
                _userDbHelper.AddNewUser(UserModel);
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
                userId = _userDbHelper.LogInUser(UserModel);
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
            return !(string.IsNullOrEmpty(UserModel.FirstName) || string.IsNullOrEmpty(UserModel.Email) || string.IsNullOrEmpty(UserModel.Password) 
                || string.IsNullOrEmpty(this.ConfirmPassword));
        }
    }
}

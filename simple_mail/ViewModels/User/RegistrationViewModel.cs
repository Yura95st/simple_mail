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
            ConfirmPassword = "";
        }

        private void CreateNewUserAccount()
        {
            if (!MyValidation.AreTwoPasswordsEqual(UserModel.Password, ConfirmPassword))
            {
                _notification.Text = "Passwords are not equal!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            try
            {
                _userDbHelper.AddNewUser(UserModel);
            }
            catch (InvalidEmailException e)
            {
                _notification.Text = "Email is invalid!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch (InvalidFirstNameException e)
            {
                _notification.Text = "Username is invalid!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch (InvalidPasswordException e)
            {
                _notification.Text = "Password must: be between 6 and 30 characters long, contain at least one number, uppercase letter, lowercase letter, special character.";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch (UserAlreadyExistsException e)
            {
                _notification.Text = "UserAccount with such email already exists!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            int userId = 0;

            try
            {
                userId = _userDbHelper.LogInUser(UserModel);
            }
            catch (Exception e)
            {
                _notification.Text = "An error occured while logging You in!";
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
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

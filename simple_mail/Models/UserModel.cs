using simple_mail.HelperClasses;
using data_models.Models;

namespace simple_mail.Models
{
    public class UserModel : ObservableObject
    {
        private User _user;

        public UserModel(User user)
        {
            _user = user;
        }

        public int Id
        {
            get {
                return _user.Id; 
            }
            set
            {
                if (value != _user.Id)
                {
                    //TODO: add userId validation
                    _user.Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string FirstName
        {
            get
            {
                return _user.FirstName;
            }
            set
            {
                if (value != _user.FirstName)
                {
                    //TODO: add FirstName validation
                    _user.FirstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public string Email
        {
            get
            {
                return _user.Email;
            }
            set
            {
                if (value != _user.Email)
                {
                    //TODO: add Email validation
                    _user.Email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public string Password
        {
            get
            {
                return _user.Password;
            }
            set
            {
                if (value != _user.Password)
                {
                    //TODO: add Password validation
                    _user.Password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        public int State
        {
            get
            {
                return _user.State;
            }
            set
            {
                if (value != _user.State)
                {
                    //TODO: add State validation
                    _user.State = value;
                    OnPropertyChanged("State");
                }
            }
        }
    }
}

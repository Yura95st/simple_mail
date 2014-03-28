using simple_mail.HelperClasses;
using data_models.Models;
using data_models.Validations;
using System;

namespace simple_mail.Models
{
    public class UserModel : ObservableObject
    {
        private User _user = new User();

        public UserModel()
        { }

        public UserModel(User user)
        {
            _user = user;
        }

        public User User
        {
            get
            {
                return _user;
            }
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
                    _user.Email = value.ToLower();
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
                    _user.State = value;
                    OnPropertyChanged("State");
                }
            }
        }
    }
}

using data_models.HelperClasses;

namespace data_models.Models
{
    public class User : ObservableObject
    {
        private int _id;
        private string _fistName;
        private string _email;
        private string _password;
        private int _state;

        public enum States
        {
            Offline = 0,
            Online = 1
        };

        public User()
        {
            _id = 0;
            _fistName = "";
            _email = "";
            _password = "";
            _state = -1;
        }

        public int Id
        {
            get {
                return _id; 
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string FirstName
        {
            get
            {
                return _fistName;
            }
            set
            {
                if (value != _fistName)
                {
                    _fistName = value.Trim();
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (value != _email)
                {
                    _email = value.Trim();
                    OnPropertyChanged("Email");
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != _password)
                {
                    _password = value.Trim();
                    OnPropertyChanged("Password");
                }
            }
        }

        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }
    }
}


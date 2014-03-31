using data_models.HelperClasses;

namespace data_models.Models
{
    public class Notification : ObservableObject
    {
        private static Notification _notification;

        private string _text;
        private int _type;
        private int _state;

        public enum States
        {            
            Hidden = 0,
            Visible = 1,
            Collapsed = 2
        }

        public enum Types
        {
            Info = 0,
            Error = 1
        }

        private Notification()
        { }

        // singleton
        public static Notification Instance
        {
            get
            {
                if (_notification == null)
                {
                    _notification = new Notification();
                }
                return _notification;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    _text = value.Trim();
                    OnPropertyChanged("Text");
                }
            }
        }

        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    OnPropertyChanged("Type");
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


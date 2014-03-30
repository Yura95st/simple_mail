using data_models.HelperClasses;
using data_models.Models;
using System;

namespace data_models.Models
{
    public class Message : ObservableObject
    {
        private int _id;
        private string _subject;
        private string _text;
        private DateTime _pubDate;
        private User _author;
        private User _recipient;
        private int _state;
        private int _recipientMsgState;

        public Message()
        { }

        public int Id
        {
            get
            {
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

        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                if (value != _subject)
                {
                    _subject = value;
                    OnPropertyChanged("Subject");
                }
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
                    _text = value.ToLower();
                    OnPropertyChanged("Text");
                }
            }
        }

        public DateTime PubDate
        {
            get
            {
                return _pubDate;
            }
            set
            {
                if (value != _pubDate)
                {
                    _pubDate = value;
                    OnPropertyChanged("PubDate");
                }
            }
        }

        public User Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (value != _author)
                {
                    _author = value;
                    OnPropertyChanged("Author");
                }
            }
        }

        public User Recipient
        {
            get
            {
                return _recipient;
            }
            set
            {
                if (value != _recipient)
                {
                    _recipient = value;
                    OnPropertyChanged("Recipient");
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

        public int RecipientMsgState
        {
            get
            {
                return _recipientMsgState;
            }
            set
            {
                if (value != _recipientMsgState)
                {
                    _recipientMsgState = value;
                    OnPropertyChanged("RecipientMsgState");
                }
            }
        }
    }
}

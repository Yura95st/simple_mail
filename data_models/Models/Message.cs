
using System;
namespace data_models.Models
{
    public class Message
    {
        public int Id
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public DateTime PubDate
        {
            get;
            set;
        }

        public int State
        {
            get;
            set;
        }

        public User Author
        {
            get;
            set;
        }
    }
}

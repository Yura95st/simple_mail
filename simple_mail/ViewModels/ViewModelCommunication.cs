using simple_mail.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simple_mail.ViewModels
{
    public static class ViewModelCommunication
    {
        static ViewModelCommunication()
        {
            Messaging = new Messenger();
        }

        //a singleton object for view model communication
        public static Messenger Messaging
        {
            get;
            set;
        }

        //list of messages
        public static string UserLoggedIn
        {
            get
            {
                return "UserLoggedIn";
            }
        }

        public static string UserLoggedOut
        {
            get
            {
                return "UserLoggedOut";
            }
        }

        public static string ReadMessage
        {
            get
            {
                return "ReadMessage";
            }
        }

        public static string SentMessage
        {
            get
            {
                return "SentMessage";
            }
        }
    }
}

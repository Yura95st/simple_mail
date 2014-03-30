namespace database_lib
{
    public static class DbValues
    {
        // Table Names
        public static string TABLE_USERS = "users";
        public static string TABLE_MESSAGES = "messages";
        public static string TABLE_MESSAGE_USERS = "message_users";
        public static string TABLE_MESSAGES_VIEW = "messages_view";

        // Table Caption Names
        public static string TABLE_CAPTION_USERS = "u";
        public static string TABLE_CAPTION_MESSAGES = "m";
        public static string TABLE_CAPTION_MESSAGE_USERS = "mu";


        // USERS table - column names
        public static string USERS_COLUMN_ID = "user_id";
        public static string USERS_COLUMN_FIRST_NAME = "first_name";
        public static string USERS_COLUMN_EMAIL = "email";
        public static string USERS_COLUMN_PASSWORD = "password";
        public static string USERS_COLUMN_STATE = "state";

        // USERS table - column caption names
        public static string USERS_CAPTION_COLUMN_ID = TABLE_USERS + "_" + USERS_COLUMN_ID;
        public static string USERS_CAPTION_COLUMN_FIRST_NAME = TABLE_USERS + "_" + USERS_COLUMN_FIRST_NAME;
        public static string USERS_CAPTION_COLUMN_EMAIL = TABLE_USERS + "_" + USERS_COLUMN_EMAIL;
        public static string USERS_CAPTION_COLUMN_PASSWORD = TABLE_USERS + "_" + USERS_COLUMN_PASSWORD;
        public static string USERS_CAPTION_COLUMN_STATE = TABLE_USERS + "_" + USERS_COLUMN_STATE;

        
        // MESSAGES table - column names
        public static string MESSAGES_COLUMN_ID = "msg_id";
        public static string MESSAGES_COLUMN_AUTHOR_ID = "author_id";
        public static string MESSAGES_COLUMN_SUBJECT = "subject";
        public static string MESSAGES_COLUMN_TEXT = "text";
        public static string MESSAGES_COLUMN_PUB_DATE = "pub_date";
        public static string MESSAGES_COLUMN_STATE = "state";

        // MESSAGES table - column caption names
        public static string MESSAGES_CAPTION_COLUMN_ID = TABLE_MESSAGES + "_" + MESSAGES_COLUMN_ID;
        public static string MESSAGES_CAPTION_COLUMN_AUTHOR_ID = TABLE_MESSAGES + "_" + MESSAGES_COLUMN_AUTHOR_ID;
        public static string MESSAGES_CAPTION_COLUMN_SUBJECT = TABLE_MESSAGES + "_" + MESSAGES_COLUMN_SUBJECT;
        public static string MESSAGES_CAPTION_COLUMN_TEXT = TABLE_MESSAGES + "_" + MESSAGES_COLUMN_TEXT;
        public static string MESSAGES_CAPTION_COLUMN_PUB_DATE = TABLE_MESSAGES + "_" + MESSAGES_COLUMN_PUB_DATE;
        public static string MESSAGES_CAPTION_COLUMN_STATE = TABLE_MESSAGES + "_" + MESSAGES_COLUMN_STATE;



        // MESSAGE_USERS table - column names
        public static string MESSAGE_USERS_COLUMN_ID = "msg_id";
        public static string MESSAGE_USERS_COLUMN_USER_ID = "user_id";
        public static string MESSAGE_USERS_COLUMN_STATE = "state";

        // MESSAGE_USERS table - column caption names
        public static string MESSAGE_USERS_CAPTION_COLUMN_ID = TABLE_MESSAGE_USERS + "_" + MESSAGE_USERS_COLUMN_ID;
        public static string MESSAGE_USERS_CAPTION_COLUMN_USER_ID = TABLE_MESSAGE_USERS + "_" + MESSAGE_USERS_COLUMN_USER_ID;
        public static string MESSAGE_USERS_CAPTION_COLUMN_STATE = TABLE_MESSAGE_USERS + "_" + MESSAGE_USERS_COLUMN_STATE;


        // MESSAGES_VIEW view - column names
        public static string MESSAGES_VIEW_COLUMN_ID = "messages_msg_id";
        public static string MESSAGES_VIEW_COLUMN_SUBJECT = "messages_subject";
        public static string MESSAGES_VIEW_COLUMN_TEXT = "messages_text";
        public static string MESSAGES_VIEW_COLUMN_PUB_DATE = "messages_pub_date";
        public static string MESSAGES_VIEW_COLUMN_STATE = "messages_state";
        public static string MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE = "messages_recipient_msg_state";
        public static string MESSAGES_VIEW_COLUMN_AUTHOR_ID = "author_id";
        public static string MESSAGES_VIEW_COLUMN_AUTHOR_FIRST_NAME = "author_first_name";
        public static string MESSAGES_VIEW_COLUMN_AUTHOR_EMAIL = "author_email";
        public static string MESSAGES_VIEW_COLUMN_AUTHOR_STATE = "author_state";
        public static string MESSAGES_VIEW_COLUMN_RECIPIENT_ID = "recipient_user_id";
        public static string MESSAGES_VIEW_COLUMN_RECIPIENT_FIRST_NAME = "recipient_first_name";
        public static string MESSAGES_VIEW_COLUMN_RECIPIENT_EMAIL = "recipient_email";
        public static string MESSAGES_VIEW_COLUMN_RECIPIENT_STATE = "recipient_state";


        // MESSAGES_VIEW select query:
        //    SELECT m.msg_id, m.subject, m.text, m.pub_date, m.state, 
        //        a.user_id AS author_id, a.first_name AS author_first_name, a.email AS author_email, a.state AS author_state,
        //        u.user_id AS recipient_id, u.first_name AS recipient_first_name, u.email AS recipient_email, u.state AS recipient_state,
        //        mu.state AS recipient_msg_state
        //    FROM messages m JOIN users a
        //        ON m.author_id = a.user_id
        //    JOIN message_users mu
        //        ON mu.msg_id = m.msg_id
        //    JOIN users u
        //        ON u.user_id = mu.user_id
    }
}

namespace database_lib
{
    public static class DbValues
    {
        // Table Names
        public static string TABLE_USERS = "users";
        public static string TABLE_MESSAGES = "messages";
        public static string TABLE_MESSAGE_USERS = "message_users";

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
    }
}

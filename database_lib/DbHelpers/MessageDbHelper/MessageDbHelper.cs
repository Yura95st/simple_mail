using data_models.Models;
using data_models.Validations;
using database_lib.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace database_lib.DbHelpers
{
    public class MessageDbHelper : BaseMessageDbHelper
    {
        private static MessageDbHelper _instance;

        /* SELECT messages_msg_id, messages_subject, messages_text, messages_pub_date, messages_state, 
               author_id, author_first_name, author_email, author_state, recipient_user_id, 
               recipient_first_name, recipient_email, recipient_state, messages_recipient_msg_state
           FROM messages_view
         */
        private string selectMessagesCommand = String.Format(@"
            SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}
            FROM {14} ",
            DbValues.MESSAGES_VIEW_COLUMN_ID,
            DbValues.MESSAGES_VIEW_COLUMN_SUBJECT,
            DbValues.MESSAGES_VIEW_COLUMN_TEXT,
            DbValues.MESSAGES_VIEW_COLUMN_PUB_DATE,
            DbValues.MESSAGES_VIEW_COLUMN_STATE,
            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE,
            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_ID,
            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_FIRST_NAME,
            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_EMAIL,
            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_STATE,
            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID,
            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_FIRST_NAME,
            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_EMAIL,
            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_STATE,
            DbValues.TABLE_MESSAGES_VIEW
        );

        private MessageDbHelper() { }

        // singleton
        public static MessageDbHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageDbHelper();
                }
                return _instance;
            }
        }

        public override List<Message> GetMessagesById(List<int> msgIdList)
        {
            if (msgIdList == null || msgIdList.Count == 0)
            {
                return null;
            }

            //convert Message id list to string
            string idString = String.Join(", ", msgIdList.FindAll(id => id > 0));

            SqlCommand cmd = new SqlCommand();

            //WHERE messages_msg_id IN (" + idString + ")
            cmd.CommandText = String.Format(@"
                {0} WHERE {1} IN ({2})",
                selectMessagesCommand, DbValues.MESSAGES_VIEW_COLUMN_ID, idString
            );

            return ExecuteSelectMessagesCommand(cmd);
        }

        public override int AddNewMessage(Message message)
        {
            try
            {
                MyValidation.CheckValidMessageFields(message);
            }
            catch
            {
                throw;
            }

            SqlCommand cmd = new SqlCommand();

            // INSERT INTO messages(author_id, subject, text, pub_date, state) 
            // VALUES(@author_id, @subject, @text, @pub_date, @state)
            cmd.CommandText = String.Format(@"
                INSERT INTO {0}({1}, {2}, {3}, {4}, {5}) 
                VALUES(@author_id, @subject, @text, @pub_date, @state); SELECT SCOPE_IDENTITY();",
                DbValues.TABLE_MESSAGES, DbValues.MESSAGES_COLUMN_AUTHOR_ID, 
                DbValues.MESSAGES_COLUMN_SUBJECT, DbValues.MESSAGES_COLUMN_TEXT, 
                DbValues.MESSAGES_COLUMN_PUB_DATE, DbValues.MESSAGES_COLUMN_STATE
            );

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@author_id",
                Value = message.Author.Id,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@subject",
                Value = message.Subject.Trim(),
                SqlDbType = SqlDbType.VarChar
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@text",
                Value = message.Text,
                SqlDbType = SqlDbType.Text
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@pub_date",
                Value = DateTime.Now,
                SqlDbType = SqlDbType.DateTime
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = (int)Message.AuthorMessageState.InitState,
                SqlDbType = SqlDbType.Int
            });

            int msgId;

            try
            {
                msgId = Convert.ToInt32(ExecuteScalarCommand(cmd).ToString());
            }
            catch
            {
                return 0;
            }

            try
            {
                this.SendMessage(msgId, message.Recipient.Id);
            }
            catch
            {
                throw;
            }            

            return msgId;
        }

        protected override void SendMessage(int msgId, int recipientId)
        {
            if (msgId <= 0)
            {
                throw new ArgumentOutOfRangeException("msgId");
            }

            if (recipientId <= 0)
            {
                throw new ArgumentOutOfRangeException("recipientId");
            }

            SqlCommand cmd = new SqlCommand();

            // INSERT INTO message_users(msg_id, user_id, state) 
            // VALUES(@msg_id, @user_id, @state)
            cmd.CommandText = String.Format(@"
                INSERT INTO {0}({1}, {2}, {3})
                VALUES(@msg_id, @user_id, @state)",
                DbValues.TABLE_MESSAGE_USERS, DbValues.MESSAGE_USERS_COLUMN_ID,
                DbValues.MESSAGE_USERS_COLUMN_USER_ID, DbValues.MESSAGE_USERS_COLUMN_STATE
            );

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@msg_id",
                Value = msgId,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@user_id",
                Value = recipientId,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = (int)Message.RecipientMessageState.Unread,
                SqlDbType = SqlDbType.Int
            });

            ExecuteNonQueryCommand(cmd);
        }

        protected override List<Message> GetMessages(int userId, int msgType)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = selectMessagesCommand;

            switch(msgType)
            {
                case (int)MessageType.Inbox:
                    {
                        /* WHERE recipient_user_id = @user_id 
                           AND (messages_recipient_msg_state = 0 OR messages_recipient_msg_state = 3)
                         */
                        cmd.CommandText += String.Format(@" 
                            WHERE {0} = @user_id AND ({1} = {2} OR {3} = {4})", 
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID,
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE, (int)Message.RecipientMessageState.Read,
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE, (int)Message.RecipientMessageState.Unread
                        );
                        break;
                    }

                case (int)MessageType.Sent:
                    {
                        /* WHERE author_id = @user_id AND messages_state = 0 */
                        cmd.CommandText += String.Format(@" 
                            WHERE {0} = @user_id AND {1} = {2}",
                            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_ID,
                            DbValues.MESSAGES_VIEW_COLUMN_STATE, (int)Message.AuthorMessageState.InitState
                        );
                        break;
                    }

                case (int)MessageType.Trash:
                    {
                        /* WHERE (author_id = @user_id AND messages_state = 1)
                           OR (recipient_user_id = @user_id AND messages_recipient_msg_state = 1)";
                         */
                        cmd.CommandText += String.Format(@" 
                            WHERE ({0} = @user_id AND {1} = {2}) 
                                OR ({3} = @user_id AND {4} = {5})",
                            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_ID, DbValues.MESSAGES_VIEW_COLUMN_STATE,
                            (int)Message.AuthorMessageState.MovedToTrash, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID,
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE, (int)Message.RecipientMessageState.MovedToTrash
                        );
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException("msgType");
            }

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@user_id",
                Value = userId,
                SqlDbType = SqlDbType.Int
            });

            return ExecuteSelectMessagesCommand(cmd);
        }


        protected override void SetAuthorMessageState(int msgId, int state)
        {
            if (msgId <= 0)
            {
                throw new ArgumentOutOfRangeException("msgId");
            }

            if (state < 0)
            {
                throw new ArgumentOutOfRangeException("state");
            }

            SqlCommand cmd = new SqlCommand();

            /* UPDATE messages SET state = @state
               WHERE msg_id = @msg_id 
             */
            cmd.CommandText = String.Format(@"
                UPDATE {0} SET {1} = @state
                WHERE {2} = @msg_id",
                DbValues.TABLE_MESSAGES, DbValues.MESSAGES_COLUMN_STATE, DbValues.MESSAGES_COLUMN_ID);

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@msg_id",
                Value = msgId,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = state,
                SqlDbType = SqlDbType.Int
            });

            ExecuteNonQueryCommand(cmd);
        }

        protected override void SetRecipientMessageState(int msgId, int recipientId, int state)
        {
            if (msgId <= 0)
            {
                throw new ArgumentOutOfRangeException("msgId");
            }

            if (recipientId <= 0)
            {
                throw new ArgumentOutOfRangeException("recipientId");
            }

            if (state < 0)
            {
                throw new ArgumentOutOfRangeException("state");
            }

            SqlCommand cmd = new SqlCommand();

            /* UPDATE message_users SET state = @state
               WHERE msg_id = @msg_id AND user_id = @user_id
             */
            cmd.CommandText = String.Format(@"
                UPDATE {0} SET {1} = @state
                WHERE {2} = @msg_id AND {3} = @user_id",
                DbValues.TABLE_MESSAGE_USERS, DbValues.MESSAGE_USERS_COLUMN_STATE,
                DbValues.MESSAGE_USERS_COLUMN_ID, DbValues.MESSAGE_USERS_COLUMN_USER_ID
            );

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@msg_id",
                Value = msgId,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@user_id",
                Value = recipientId,
                SqlDbType = SqlDbType.Int
            });

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@state",
                Value = state,
                SqlDbType = SqlDbType.Int
            });

            ExecuteNonQueryCommand(cmd);
        }

        // returns message object from the query result - dataReader
        public static Message GetMessageFromQueryResult(DbDataReader dataReader)
        {
            Message message = null;

            if (dataReader != null)
            {
                message = new Message();

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_ID))
                {
                    message.Id = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_ID]);
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_SUBJECT))
                {
                    message.Subject = dataReader[DbValues.MESSAGES_VIEW_COLUMN_SUBJECT].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_TEXT))
                {
                    message.Text = dataReader[DbValues.MESSAGES_VIEW_COLUMN_TEXT].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_PUB_DATE))
                {
                    message.PubDate = Convert.ToDateTime(dataReader[DbValues.MESSAGES_VIEW_COLUMN_PUB_DATE].ToString());
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_STATE))
                {
                    message.State = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_STATE]);
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE))
                {
                    message.RecipientMsgState = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE]);
                }

                message.Author = GetMessageAuthorFromQueryResult(dataReader);
                message.Recipient = GetMessageRecipientFromQueryResult(dataReader);
            }

            return message;
        }

        // returns message author object from the query result - dataReader
        public static User GetMessageAuthorFromQueryResult(DbDataReader dataReader)
        {
            User author = null;

            if (dataReader != null)
            {
                author = new User();

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_ID))
                {
                    author.Id = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_ID]);
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_FIRST_NAME))
                {
                    author.FirstName = dataReader[DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_FIRST_NAME].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_EMAIL))
                {
                    author.Email = dataReader[DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_EMAIL].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_STATE))
                {
                    author.State = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_STATE]);
                }
            }

            return author;
        }

        // returns message recipient object from the query result - dataReader
        public static User GetMessageRecipientFromQueryResult(DbDataReader dataReader)
        {
            User recipient = null;

            if (dataReader != null)
            {
                recipient = new User();

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID))
                {
                    recipient.Id = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID]);
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_FIRST_NAME))
                {
                    recipient.FirstName = dataReader[DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_FIRST_NAME].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_EMAIL))
                {
                    recipient.Email = dataReader[DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_EMAIL].ToString();
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_STATE))
                {
                    recipient.State = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_STATE]);
                }
            }

            return recipient;
        }

        // executes sql command to get messages
        private List<Message> ExecuteSelectMessagesCommand(SqlCommand command)
        {
            List<Message> messages = new List<Message>();

            try
            {
                connection.Open();

                //bind opened connection to the command
                command.Connection = connection;

                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        messages.Add(GetMessageFromQueryResult(dataReader));
                    }
                }
            }
            catch (SqlException ex)
            { }
            finally
            {
                connection.Close();
            }

            //orders message list by PubDate
            return messages.OrderByDescending(m => m.PubDate).ToList();
        }
    }
}

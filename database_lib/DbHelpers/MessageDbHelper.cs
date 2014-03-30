using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using database_lib.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace database_lib.DbHelpers
{
    public class MessageDbHelper : BaseDbHelper
    {
        public enum AuthorMessageState
        {
            InitState = 0,
            MovedToTrash = 1,
            Deleted = 2
        };

        public enum RecipientMessageState
        {
            Read = 0,
            MovedToTrash = 1,
            Deleted = 2,
            Unread = 3
        };

        private enum MessageType
        {
            Inbox,
            Sent,
            Trash
        };

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

        public MessageDbHelper()
        {
        }

        // gets message by it's id number
        public Message GetMessageById(int msgId)
        {
            if (msgId <= 0)
            {
                throw new ArgumentOutOfRangeException("msgId");
            }

            List<int> msgIdList = new List<int>();
            msgIdList.Add(msgId);

            Message msg = null;

            try
            {
                msg = GetMessagesById(msgIdList)[0];
            }
            catch
            {
                return null;
            }

            return msg;
        }

        // gets messages with id numbers from "msgIdList" list
        public List<Message> GetMessagesById(List<int> msgIdList)
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

        // adds new message
        // returns id of the newly created message or 0 - if it wasn't created
        public int AddNewMessage(Message message)
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
                Value = message.Subject,
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
                Value = (int)AuthorMessageState.InitState,
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

        private void SendMessage(int msgId, int recipientId)
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
                Value = (int)RecipientMessageState.Unread,
                SqlDbType = SqlDbType.Int
            });

            ExecuteNonQueryCommand(cmd);
        }

        // gets inbox messages
        public List<Message> GetInboxMessages(int userId)
        {
            return GetMessages(userId, (int)MessageType.Inbox);
        }

        // gets sent messages
        public List<Message> GetSentMessages(int userId)
        {
            return GetMessages(userId, (int)MessageType.Sent);
        }

        // gets messages moved to trash
        public List<Message> GetTrashMessages(int userId)
        {
            return GetMessages(userId, (int)MessageType.Trash);
        }

        // gets messages by type: inbox, sent, trashed
        private List<Message> GetMessages(int userId, int msgType)
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
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE, (int)RecipientMessageState.Read, 
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_MSG_STATE, (int)RecipientMessageState.Unread
                        );
                        break;
                    }

                case (int)MessageType.Sent:
                    {
                        /* WHERE author_id = @user_id AND messages_state = 0 */
                        cmd.CommandText += String.Format(@" 
                            WHERE {0} = @user_id AND {1} = {2}",
                            DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_ID, 
                            DbValues.MESSAGES_VIEW_COLUMN_STATE, (int)AuthorMessageState.InitState
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
                            (int)AuthorMessageState.MovedToTrash, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID,
                            DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_STATE, (int)RecipientMessageState.MovedToTrash
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

        // restores message, added by author
        public void RestoreAuthorMessage(int msgId)
        {
            SetAuthorMessageState(msgId, (int)AuthorMessageState.InitState);
        }

        // restores message, recieved by recipient
        public void RestoreRecipientMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)RecipientMessageState.Read);
        }

        // marks message as read
        public void ReadMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)RecipientMessageState.Read);
        }
        
        // marks message as unread
        public void MarkMessageAsUnread(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)RecipientMessageState.Unread);
        }

        // moves to trash message, added by author
        public void MoveToTrashAuthorMessage(int msgId)
        {
            SetAuthorMessageState(msgId, (int)AuthorMessageState.MovedToTrash);
        }

        // moves to trash message, recieved by recipient
        public void MoveToTrashRecipientMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)RecipientMessageState.MovedToTrash);
        }

        // deletes message, added by author
        public void DeleteAuthorMessage(int msgId)
        {
            SetAuthorMessageState(msgId, (int)AuthorMessageState.Deleted);
        }

        // deletes message, recieved by recipient
        public void DeleteRecipientMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)RecipientMessageState.Deleted);
        }

        // returns message object from the query result - dataReader
        public static Message GetMessageFromQueryResult(DbDataReader dataReader)
        {
            Message message = new Message();

            if (dataReader != null)
            {
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
            User author = new User();

            if (dataReader != null)
            {
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
            User recipient = new User();

            if (dataReader != null)
            {
                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID))
                {
                    recipient.Id = Convert.ToInt32(dataReader[DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_ID]);
                }

                if (DbValidation.ColumnExists(dataReader, DbValues.MESSAGES_VIEW_COLUMN_RECIPIENT_FIRST_NAME))
                {
                    recipient.FirstName = dataReader[DbValues.MESSAGES_VIEW_COLUMN_AUTHOR_FIRST_NAME].ToString();
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

            return messages;
        }

        // changes author message state: 0 - init state, 1 - moved to trash, 2 - deleted
        private void SetAuthorMessageState(int msgId, int state)
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

        // changes recipient message state: 0 - read, 1 - moved to trash, 2 - deleted, 3 - unread
        private void SetRecipientMessageState(int msgId, int recipientId, int state)
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
    }
}

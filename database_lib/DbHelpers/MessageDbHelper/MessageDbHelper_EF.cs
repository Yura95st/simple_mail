using data_models.Models;
using System.Collections.Generic;
using database_lib.EntityDbDataModel;
using System.Linq;
using System;
using data_models.Validations;

namespace database_lib.DbHelpers
{
    public class MessageDbHelper_EF : BaseMessageDbHelper
    {
        private static MessageDbHelper_EF _instance;

        private MessageDbHelper_EF() { }

        // singleton
        public static MessageDbHelper_EF Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageDbHelper_EF();
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

            List<Message> msgList = new List<Message>();

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var msgEntityList = (from mu in db.messages_view
                                          where msgIdList.Contains(mu.messages_msg_id)
                                          select mu).ToList();

                    foreach (var msgEntity in msgEntityList)
                    {
                        msgList.Add(GetMessageFromDataEntity(msgEntity));
                    }
                }
                catch (ArgumentNullException)
                {
                    return null;
                }
            }

            //orders message list by PubDate
            return msgList.OrderByDescending(m => m.PubDate).ToList();
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

            int msgId;

            using (var db = new simple_mailEntities())
            {
                message msgEntity = new message();

                msgEntity.author_id = message.Author.Id;
                msgEntity.subject = message.Subject.Trim();
                msgEntity.text = message.Text;
                msgEntity.pub_date = DateTime.Now;
                msgEntity.state = (int)Message.AuthorMessageState.InitState;

                try
                {
                    db.messages.Add(msgEntity);
                    db.SaveChanges();

                    msgId = msgEntity.msg_id;
                }
                catch (InvalidOperationException e)
                {
                    return 0;
                }
            }

            if (msgId != 0)
            {
                try
                {
                    this.SendMessage(msgId, message.Recipient.Id);
                }
                catch
                {
                    throw;
                }
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

            using (var db = new simple_mailEntities())
            {
                message_users msgUsersEntity = new message_users();

                msgUsersEntity.msg_id = msgId;
                msgUsersEntity.user_id = recipientId;
                msgUsersEntity.state = (int)Message.RecipientMessageState.Unread;

                try
                {
                    db.message_users.Add(msgUsersEntity);
                    db.SaveChanges();
                }
                catch (InvalidOperationException e)
                {
                    return;
                }
            }
        }

        protected override List<Message> GetMessages(int userId, int msgType)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            List<Message> msgList = new List<Message>();

            IQueryable<messages_view> query;

            using (var db = new simple_mailEntities())
            {
                try
                {
                    query = (from mv in db.messages_view
                             select mv);

                    switch (msgType)
                    {
                        case (int)MessageType.Inbox:
                            {
                                query = query.Where(
                                    mv => mv.recipient_user_id == userId && (mv.messages_recipient_msg_state == (int)Message.RecipientMessageState.Read || mv.messages_recipient_msg_state == (int)Message.RecipientMessageState.Unread)
                                );
                                break;
                            }

                        case (int)MessageType.Sent:
                            {
                                query = query.Where(
                                    mv => mv.author_id == userId && mv.messages_state == (int)Message.AuthorMessageState.InitState
                                );
                                break;
                            }

                        case (int)MessageType.Trash:
                            {
                                query = query.Where(
                                    mv => (mv.author_id == userId && mv.messages_state == (int)Message.AuthorMessageState.MovedToTrash)
                                        || (mv.recipient_user_id == userId && mv.messages_recipient_msg_state == (int)Message.RecipientMessageState.MovedToTrash)
                                );
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException("msgType");
                    }
                }
                catch (ArgumentNullException e)
                {
                    return msgList;
                }

                var messages = query.ToList();

                foreach (var msg in messages)
                {
                    msgList.Add(GetMessageFromDataEntity(msg));
                }
            }

            //orders message list by PubDate
            return msgList.OrderByDescending(m => m.PubDate).ToList();
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

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var msgEntity = (from m in db.messages
                                      where m.msg_id == msgId
                                      select m).SingleOrDefault();

                    if (msgEntity == null)
                    {
                        return;
                    }

                    msgEntity.state = state;

                    db.SaveChanges();
                }
                catch (ArgumentNullException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
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

            using (var db = new simple_mailEntities())
            {
                try
                {
                    var msgUserEntity = (from mu in db.message_users
                                         where mu.msg_id == msgId && mu.user_id == recipientId
                                         select mu).SingleOrDefault();

                    if (msgUserEntity == null)
                    {
                        return;
                    }

                    msgUserEntity.state = state;

                    db.SaveChanges();
                }
                catch (ArgumentNullException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }
        }

        // returns message object from data (messages_view)
        public static Message GetMessageFromDataEntity(messages_view data)
        {
            Message message = null;

            if (data != null)
            {
                message = new Message();

                message.Id = data.messages_msg_id;
                message.Subject = data.messages_subject;
                message.Text = data.messages_text;
                message.PubDate = data.messages_pub_date;
                message.State = data.messages_state;
                message.RecipientMsgState = data.messages_recipient_msg_state;

                message.Author = GetMessageAuthorFromDataEntity(data);
                message.Recipient = GetMessageRecipientFromDataEntity(data);
            }

            return message;
        }

        // returns message author object from data
        public static User GetMessageAuthorFromDataEntity(messages_view data)
        {
            User author = null;

            if (data != null)
            {
                author = new User();

                author.Id = data.author_id;
                author.FirstName = data.author_first_name;
                author.Email = data.author_email;
                author.State = data.author_state;
            }

            return author;
        }

        // returns message recipient object from data
        public static User GetMessageRecipientFromDataEntity(messages_view data)
        {
            User recipient = null;

            if (data != null)
            {
                recipient = new User();

                recipient.Id = data.recipient_user_id;
                recipient.FirstName = data.recipient_first_name;
                recipient.Email = data.recipient_email;
                recipient.State = data.recipient_state;
            }

            return recipient;
        }
    }
}

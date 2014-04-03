using data_models.Models;
using System;
using System.Collections.Generic;

namespace database_lib.DbHelpers
{
    public abstract class BaseMessageDbHelper : BaseDbHelper, IMessageDbHelper
    {
        protected enum MessageType
        {
            Inbox,
            Sent,
            Trash
        };        

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

        public abstract List<Message> GetMessagesById(List<int> msgIdList);

        public abstract int AddNewMessage(Message message);

        protected abstract void SendMessage(int msgId, int recipientId);

        public List<Message> GetInboxMessages(int userId)
        {
            return GetMessages(userId, (int)MessageType.Inbox);
        }

        public List<Message> GetSentMessages(int userId)
        {
            return GetMessages(userId, (int)MessageType.Sent);
        }

        public List<Message> GetTrashMessages(int userId)
        {
            return GetMessages(userId, (int)MessageType.Trash);
        }

        protected abstract List<Message> GetMessages(int userId, int msgType);

        public void RestoreAuthorMessage(int msgId)
        {
            SetAuthorMessageState(msgId, (int)Message.AuthorMessageState.InitState);
        }

        public void RestoreRecipientMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)Message.RecipientMessageState.Read);
        }

        public void ReadMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)Message.RecipientMessageState.Read);
        }
        
        public void MarkMessageAsUnread(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)Message.RecipientMessageState.Unread);
        }

        public void MoveToTrashAuthorMessage(int msgId)
        {
            SetAuthorMessageState(msgId, (int)Message.AuthorMessageState.MovedToTrash);
        }

        public void MoveToTrashRecipientMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)Message.RecipientMessageState.MovedToTrash);
        }

        public void DeleteAuthorMessage(int msgId)
        {
            SetAuthorMessageState(msgId, (int)Message.AuthorMessageState.Deleted);
        }

        public void DeleteRecipientMessage(int msgId, int recipientId)
        {
            SetRecipientMessageState(msgId, recipientId, (int)Message.RecipientMessageState.Deleted);
        }

        protected abstract void SetAuthorMessageState(int msgId, int state);

        protected abstract void SetRecipientMessageState(int msgId, int recipientId, int state);
    }
}

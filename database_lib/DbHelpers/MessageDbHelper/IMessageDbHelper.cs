using data_models.Models;
using System.Collections.Generic;

namespace database_lib.DbHelpers
{
    public interface IMessageDbHelper
    {
        // gets message by it's id number
        Message GetMessageById(int msgId);

        // gets messages with id numbers from "msgIdList" list
        List<Message> GetMessagesById(List<int> msgIdList);

        // adds new message
        // returns id of the newly created message or 0 - if it wasn't created
        int AddNewMessage(Message message);

        // gets inbox messages
        List<Message> GetInboxMessages(int userId);

        // gets sent messages
        List<Message> GetSentMessages(int userId);

        // gets messages moved to trash
        List<Message> GetTrashMessages(int userId);

        // restores message, added by author
        void RestoreAuthorMessage(int msgId);

        // restores message, recieved by recipient
        void RestoreRecipientMessage(int msgId, int recipientId);

        // marks message as read
        void ReadMessage(int msgId, int recipientId);
        
        // marks message as unread
        void MarkMessageAsUnread(int msgId, int recipientId);

        // moves to trash message, added by author
        void MoveToTrashAuthorMessage(int msgId);

        // moves to trash message, recieved by recipient
        void MoveToTrashRecipientMessage(int msgId, int recipientId);

        // deletes message, added by author
        void DeleteAuthorMessage(int msgId);

        // deletes message, recieved by recipient
        void DeleteRecipientMessage(int msgId, int recipientId);
    }
}

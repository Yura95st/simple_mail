using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System.Collections.Generic;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class InboxMessagesViewModel : BaseViewModel, IPageViewModel
    {
        private List<Message> _messagesList = new List<Message>();
        private MessageDbHelper _messageDbHelper = MessageDbHelper.Instance;

        private ICommand _markMsgAsUnreadCommand;
        private ICommand _moveMsgToTrashCommand;
        private ICommand _readMsgCommand;

        public InboxMessagesViewModel()
        {
        }

        public List<Message> MessagesList
        {
            get
            {
                return _messagesList;
            }
            set
            {
                if (value != _messagesList)
                {
                    _messagesList = value;
                    OnPropertyChanged("MessagesList");
                }
            }
        }

        public ICommand MoveMsgToTrashCommand
        {
            get
            {
                if (_moveMsgToTrashCommand == null)
                {
                    _moveMsgToTrashCommand = new RelayCommand(
                        param => MoveMessageToTrash((Message)param),
                        param => IsMessageValid((Message)param)
                    );
                }
                return _moveMsgToTrashCommand;
            }
        }

        public ICommand MarkMsgAsUnreadCommand
        {
            get
            {
                if (_markMsgAsUnreadCommand == null)
                {
                    _markMsgAsUnreadCommand = new RelayCommand(
                        param => MarkMessageAsUnread((Message)param),
                        param => IsMessageValid((Message)param) && IsMessageRead((Message)param)
                    );
                }
                return _markMsgAsUnreadCommand;
            }
        }

        public ICommand ReadMsgCommand
        {
            get
            {
                if (_readMsgCommand == null)
                {
                    _readMsgCommand = new RelayCommand(
                        param => ReadMessage((Message)param),
                        param => IsMessageValid((Message)param)
                    );
                }
                return _readMsgCommand;
            }
        }

        private void ReadMessage(Message msg)
        {
            ReadMessageViewModel.ReadMessageId = msg.Id;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.ReadMessage);
        }

        private bool IsMessageRead(Message msg)
        {
            return msg.RecipientMsgState == (int)MessageDbHelper.RecipientMessageState.Read;
        }

        private void MarkMessageAsUnread(Message msg)
        {
            try
            {
                _messageDbHelper.MarkMessageAsUnread(msg.Id, AuthorizationViewModel.LoggedUserId);
            }
            catch 
            {
                return;
            }

            msg.RecipientMsgState = (int)MessageDbHelper.RecipientMessageState.Unread;
        }

        private void MoveMessageToTrash(Message msg)
        {
            try
            {
                _messageDbHelper.MoveToTrashRecipientMessage(msg.Id, AuthorizationViewModel.LoggedUserId);
            }
            catch
            {
                return;
            }

            List<Message> newMessagesList = new List<Message>(MessagesList);
            newMessagesList.Remove(msg);
            MessagesList = newMessagesList;
        }

        private bool IsMessageValid(Message msg)
        {
            return (msg != null);
        }

        public void OnShow()
        {
            GetInbox();
        }

        private void GetInbox()
        {
            try
            {
                MessagesList = _messageDbHelper.GetInboxMessages(AuthorizationViewModel.LoggedUserId);
            }
            catch
            {
                //TODO: handle all possible exceptions
                return;
            }
        }
    }
}

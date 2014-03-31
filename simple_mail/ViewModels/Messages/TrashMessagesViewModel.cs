using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class TrashMessagesViewModel : BaseViewModel, IPageViewModel
    {
        private List<Message> _messagesList = new List<Message>();
        private MessageDbHelper _messageDbHelper = MessageDbHelper.Instance;

        private ICommand _restoreMsgCommand;
        private ICommand _deleteMsgCommand;

        public TrashMessagesViewModel()
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

        public ICommand RestoreMsgCommand
        {
            get
            {
                if (_restoreMsgCommand == null)
                {
                    _restoreMsgCommand = new RelayCommand(
                        param => RestoreMessage((Message)param),
                        param => IsMessageValid((Message)param)
                    );
                }
                return _restoreMsgCommand;
            }
        }

        public ICommand DeleteMsgCommand
        {
            get
            {
                if (_deleteMsgCommand == null)
                {
                    _deleteMsgCommand = new RelayCommand(
                        param => DeleteMessage((Message)param),
                        param => IsMessageValid((Message)param)
                    );
                }
                return _deleteMsgCommand;
            }
        }

        private void DeleteMessage(Message msg)
        {
            try
            {
                if (msg.Author.Id == AuthorizationViewModel.LoggedUserId)
                {
                    _messageDbHelper.DeleteAuthorMessage(msg.Id);
                }
                else if (msg.Recipient.Id == AuthorizationViewModel.LoggedUserId)
                {
                    _messageDbHelper.DeleteRecipientMessage(msg.Id, AuthorizationViewModel.LoggedUserId);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                return;
            }

            List<Message> newMessagesList = new List<Message>(MessagesList);
            newMessagesList.Remove(msg);
            MessagesList = newMessagesList;
        }

        private void RestoreMessage(Message msg)
        {
            try
            {
                if (msg.Author.Id == AuthorizationViewModel.LoggedUserId)
                {
                    _messageDbHelper.RestoreAuthorMessage(msg.Id);
                }
                else if (msg.Recipient.Id == AuthorizationViewModel.LoggedUserId)
                {
                    _messageDbHelper.RestoreRecipientMessage(msg.Id, AuthorizationViewModel.LoggedUserId);
                }
            }
            catch (ArgumentOutOfRangeException e)
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
            GetTrashMessages();
        }

        private void GetTrashMessages()
        {
            try
            {
                MessagesList = _messageDbHelper.GetTrashMessages(AuthorizationViewModel.LoggedUserId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return;
            }
        }
    }
}


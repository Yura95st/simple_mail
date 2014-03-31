using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class SentMessagesViewModel : BaseViewModel, IPageViewModel
    {
        private List<Message> _messagesList = new List<Message>();
        private MessageDbHelper _messageDbHelper = MessageDbHelper.Instance;

        private ICommand _moveMsgToTrashCommand;

        public SentMessagesViewModel()
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

        private void MoveMessageToTrash(Message msg)
        {
            try
            {
                _messageDbHelper.MoveToTrashAuthorMessage(msg.Id);
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
            GetSentMessages();
        }

        private void GetSentMessages()
        {
            try
            {
                MessagesList = _messageDbHelper.GetSentMessages(AuthorizationViewModel.LoggedUserId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return;
            }
        }
    }
}


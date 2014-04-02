using data_models.Models;
using database_lib;
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
        private IMessageDbHelper _messageDbHelper = DbHelpersFactory.GetMessageDbHelper(GlobalValues.HelpersTechnologyType);

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
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            List<Message> newMessagesList = new List<Message>(MessagesList);
            newMessagesList.Remove(msg);
            MessagesList = newMessagesList;

            _notification.Text = "Message was moved to trash";
            _notification.Type = (int)Notification.Types.Info;
            ApplicationViewModel.ShowNotificationBox(_notification);
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

                if (MessagesList == null || MessagesList.Count == 0)
                {
                    _notification.Text = "Sent is empty.";
                    _notification.Type = (int)Notification.Types.Info;
                    ApplicationViewModel.ShowNotificationBox(_notification);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
        }
    }
}


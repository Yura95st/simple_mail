﻿using data_models.Models;
using database_lib;
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
        private IMessageDbHelper _messageDbHelper = DbHelpersFactory.GetMessageDbHelper(GlobalValues.HelpersTechnologyType);

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
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            List<Message> newMessagesList = new List<Message>(MessagesList);
            newMessagesList.Remove(msg);
            MessagesList = newMessagesList;

            _notification.Text = "Message was deleted.";
            _notification.Type = (int)Notification.Types.Info;
            ApplicationViewModel.ShowNotificationBox(_notification);
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
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            List<Message> newMessagesList = new List<Message>(MessagesList);
            newMessagesList.Remove(msg);
            MessagesList = newMessagesList;

            _notification.Text = "Message was restored.";
            _notification.Type = (int)Notification.Types.Info;
            ApplicationViewModel.ShowNotificationBox(_notification);
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
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            if (MessagesList == null || MessagesList.Count == 0)
            {
                _notification.Text = "Trash is empty.";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
            }
        }
    }
}


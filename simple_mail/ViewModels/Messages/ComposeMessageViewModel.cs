﻿using data_models.Exceptions;
using data_models.Models;
using database_lib;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class ComposeMessageViewModel : BaseViewModel, IPageViewModel
    {
        private Message _messageModel = new Message();
        private IMessageDbHelper _messageDbHelper = DbHelpersFactory.GetMessageDbHelper(GlobalValues.HelpersTechnologyType);
        private IUserDbHelper _userDbHelper = DbHelpersFactory.GetUserDbHelper(GlobalValues.HelpersTechnologyType);

        private ICommand _sendMessageCommand;

        public ComposeMessageViewModel()
        {
        }

        public Message MessageModel
        {
            get
            {
                return _messageModel;
            }
            set
            {
                if (value != _messageModel)
                {
                    _messageModel = value;
                    OnPropertyChanged("MessageModel");
                }
            }
        }

        public ICommand SendMessageCommand
        {
            get
            {
                if (_sendMessageCommand == null)
                {
                    _sendMessageCommand = new RelayCommand(
                        param => SendMessage(),
                        param => IsMessageValid()
                    );
                }
                return _sendMessageCommand;
            }
        }

        private bool IsMessageValid()
        {
            if (string.IsNullOrEmpty(MessageModel.Subject.Trim()) || string.IsNullOrEmpty(MessageModel.Text.Trim())
                || MessageModel.Author.Id <= 0 || string.IsNullOrEmpty(MessageModel.Recipient.Email.Trim()))
            {
                return false;
            }

            return true;
        }

        private void SendMessage()
        {
            User recipient = null;
            try 
            {
                recipient = _userDbHelper.GetUserByEmail(MessageModel.Recipient.Email);
            }
            catch (InvalidEmailException e)
            {
                _notification.Text = "Email is invalid!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            if (recipient == null) 
            {
                _notification.Text = "There is no user with this email!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            MessageModel.Recipient = recipient;

            try
            {
                _messageDbHelper.AddNewMessage(MessageModel);
            }
            catch(EmtpyMessageSubjectException e)
            {
                _notification.Text = "Subject field is empty!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch(EmptyMessageTextException e)
            {
                _notification.Text = "Text field is empty!";
                _notification.Type = (int)Notification.Types.Info;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch(InvalidMessageAuthorException e)
            {
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
            catch(InvalidMessageRecipientException e)
            {
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                _notification.Type = (int)Notification.Types.Error;
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }

            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.SentMessage);

            _notification.Text = "Message was sent!";
            _notification.Type = (int)Notification.Types.Info;
            ApplicationViewModel.ShowNotificationBox(_notification);
        }

        public void OnShow()
        {
            MessageModel.Author.Id = AuthorizationViewModel.LoggedUserId;
            MessageModel.Recipient.Email = "";
            MessageModel.Subject = "";
            MessageModel.Text = "";
        }
    }
}



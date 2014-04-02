using data_models.Exceptions;
using data_models.Models;
using data_models.Validations;
using database_lib;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class ReadMessageViewModel : BaseViewModel, IPageViewModel
    {
        private Message _messageModel = new Message();
        private IMessageDbHelper _messageDbHelper = DbHelpersFactory.GetMessageDbHelper(GlobalValues.HelpersTechnologyType);
        private string _replyMessageText;

        private ICommand _replyMessageCommand;

        public static int ReadMessageId = 0;

        public ReadMessageViewModel()
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

        public string ReplyMessageText
        {
            get
            {
                return _replyMessageText;
            }
            set
            {
                if (value != _replyMessageText)
                {
                    _replyMessageText = value;
                    OnPropertyChanged("ReplyMessageText");
                }
            }
        }

        public ICommand ReplyMessageCommand
        {
            get
            {
                if (_replyMessageCommand == null)
                {
                    _replyMessageCommand = new RelayCommand(
                        param => ReplyMessage(),
                        param => IsReplyTextNotEmpty()
                    );
                }
                return _replyMessageCommand;
            }
        }

        private bool IsReplyTextNotEmpty()
        {
            return !string.IsNullOrEmpty(ReplyMessageText);
        }

        private void ReplyMessage()
        {
            Message msg = new Message();

            if (MessageModel.Author.Id == AuthorizationViewModel.LoggedUserId)
            {
                msg.Author = MessageModel.Author;
                msg.Recipient = MessageModel.Recipient;
            }
            else if (MessageModel.Recipient.Id == AuthorizationViewModel.LoggedUserId)
            {
                msg.Author = MessageModel.Recipient;
                msg.Recipient = MessageModel.Author;
            }
            else 
            {
                return;
            }

            msg.Subject = MyValidation.CreateReplySubject(MessageModel.Subject);
            msg.Text = ReplyMessageText;

            try
            {
                _messageDbHelper.AddNewMessage(msg);
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

            _notification.Text = "Reply message was sent!";
            _notification.Type = (int)Notification.Types.Info;
            ApplicationViewModel.ShowNotificationBox(_notification);
        }

        public void OnShow()
        {
            OpenMessage();
        }

        private void OpenMessage()
        {
            try
            {
                MessageModel = _messageDbHelper.GetMessageById(ReadMessageId);

                if (MessageModel.Recipient.Id == AuthorizationViewModel.LoggedUserId &&
                    MessageModel.RecipientMsgState == (int)Message.RecipientMessageState.Unread)
                {
                    // Set message's state to "Read"
                    _messageDbHelper.ReadMessage(ReadMessageId, AuthorizationViewModel.LoggedUserId);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                _notification.Text = string.Format("An error occured. Please, try again later.{0}", "DEBUG: " + e.Message);
                ApplicationViewModel.ShowNotificationBox(_notification);
                return;
            }
        }
    }
}


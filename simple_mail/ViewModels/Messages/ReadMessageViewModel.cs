using data_models.Exceptions;
using data_models.Models;
using database_lib.DbHelpers;
using simple_mail.HelperClasses;
using System;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class ReadMessageViewModel : BaseViewModel, IPageViewModel
    {
        private Message _messageModel = new Message();
        private MessageDbHelper _messageDbHelper = MessageDbHelper.Instance;
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
            return !string.Equals(ReplyMessageText, "");
        }

        private void ReplyMessage()
        {
            Message msg = new Message();

            msg.Author = MessageModel.Recipient;
            msg.Recipient = MessageModel.Author;
            msg.Subject = string.Format("Re: {0}", MessageModel.Subject);
            msg.Text = ReplyMessageText;

            try
            {
                _messageDbHelper.AddNewMessage(msg);
            }
            catch(EmtpyMessageSubjectException e)
            {
                return; 
            }
            catch(EmptyMessageTextException e)
            {
                return; 
            }
            catch(InvalidMessageAuthorException e)
            {
                return; 
            }
            catch(InvalidMessageRecipientException e)
            {
                return; 

            }

            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.SentMessage);
        }

        public void OnShow()
        {
            try
            {
                MessageModel = _messageDbHelper.GetMessageById(ReadMessageId);

                // Set message's state to "Read"
                _messageDbHelper.ReadMessage(ReadMessageId, AuthorizationViewModel.LoggedUserId);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return;
            }
        }
    }
}


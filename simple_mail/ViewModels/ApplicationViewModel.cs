using data_models.Models;
using simple_mail.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace simple_mail.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {
        #region Fields

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        private ICommand _changePageCommand;

        #endregion

        public ApplicationViewModel()
        {
            // list of available pages
            InitAvailablePages();

            // register to messages from pageViewModels
            RegisterToMessagesFromPageViewModels();

            // Set starting page
            this.ChangeViewModel(AuthorizationViewModel);
        }

        #region Properties / Commands

        public IPageViewModel AuthorizationViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is AuthorizationViewModel));
            }
        }

        public IPageViewModel RegistrationViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is RegistrationViewModel));
            }
        }

        public IPageViewModel InboxMessagesViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is InboxMessagesViewModel));
            }
        }

        public IPageViewModel SentMessagesViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is SentMessagesViewModel));
            }
        }

        public IPageViewModel TrashMessagesViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is TrashMessagesViewModel));
            }
        }

        public IPageViewModel ReadMessageViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is ReadMessageViewModel));
            }
        }

        public IPageViewModel ComposeMessageViewModel
        {
            get
            {
                return _pageViewModels.Find(p => (p is ComposeMessageViewModel));
            }
        }

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => (p is IPageViewModel) && p != CurrentPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public Notification NotificationBox
        {
            get 
            {
                return _notification;
            }
            set 
            {
                if (_notification != value)
                {
                    _notification = value;
                    OnPropertyChanged("NotificationBox");
                }
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!_pageViewModels.Contains(viewModel))
            {
                _pageViewModels.Add(viewModel);
            }

            CurrentPageViewModel = _pageViewModels.FirstOrDefault(vm => vm == viewModel);

            HideNotificationBox();

            viewModel.OnShow();
        }

        private void InitAvailablePages()
        {
            _pageViewModels = new List<IPageViewModel>();

            // Add available pages
            _pageViewModels.Add(new AuthorizationViewModel());
            _pageViewModels.Add(new RegistrationViewModel());
            
            _pageViewModels.Add(new InboxMessagesViewModel());
            _pageViewModels.Add(new SentMessagesViewModel());
            _pageViewModels.Add(new TrashMessagesViewModel());
            
            _pageViewModels.Add(new ReadMessageViewModel());
            _pageViewModels.Add(new ComposeMessageViewModel());
        }

        private void RegisterToMessagesFromPageViewModels()
        {
            RegisterToLogInUserMessage();
            RegisterToLogOutUserMessage();
            RegisterToReadMessageMessage();
            RegisterToSentMessageMessage();

            // notifications about error/info from ViewModels will be shown in the NotificationBox
            RegisterToNotificationMessage();
        }

        private void RegisterToLogInUserMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.UserLoggedIn, (Action)delegate()
            {
                this.ChangeViewModel(InboxMessagesViewModel);
            });
        }

        private void RegisterToLogOutUserMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.UserLoggedOut, (Action)delegate()
            {
                this.ChangeViewModel(AuthorizationViewModel);
            });
        }

        private void RegisterToReadMessageMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.ReadMessage, (Action)delegate()
            {
                this.ChangeViewModel(ReadMessageViewModel);
            });
        }

        private void RegisterToSentMessageMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.SentMessage, (Action)delegate()
            {
                this.ChangeViewModel(InboxMessagesViewModel);
            });
        }

        private void RegisterToNotificationMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.Notification, (Action<Notification>)delegate(Notification notification)
            {
                NotificationBox = notification;
            });
        }

        public static void ShowNotificationBox(Notification notification)
        {
            notification.State = (int)Notification.States.Visible;
            ViewModelCommunication.Messaging.NotifyColleagues(ViewModelCommunication.Notification, notification);
        }

        private void HideNotificationBox()
        {
            NotificationBox.Text = "";
            NotificationBox.State = (int)Notification.States.Collapsed;
        }

        #endregion
    }
}

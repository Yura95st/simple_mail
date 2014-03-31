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

        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        private Notification _notificationBox = Notification.Instance;

        #endregion

        public ApplicationViewModel()
        {
            // register to messages from pageViewModels
            RegisterToLogInUserMessage();
            RegisterToLogOutUserMessage();
            RegisterToReadMessageMessage();
            RegisterToSentMessageMessage();

            // notifications about error/info from ViewModels will be shown in the NotificationBox
            RegisterToNotificationMessage();

            // list of available pages
            AuthorizationViewModel = new AuthorizationViewModel();
            RegistrationViewModel = new RegistrationViewModel();

            InboxMessagesViewModel = new InboxMessagesViewModel();
            SentMessagesViewModel = new SentMessagesViewModel();
            TrashMessagesViewModel = new TrashMessagesViewModel();

            ReadMessageViewModel = new ReadMessageViewModel();
            ComposeMessageViewModel = new ComposeMessageViewModel();
            

            // Add available pages
            PageViewModels.Add(this.AuthorizationViewModel);
            PageViewModels.Add(this.RegistrationViewModel);

            PageViewModels.Add(this.InboxMessagesViewModel);
            PageViewModels.Add(this.SentMessagesViewModel);
            PageViewModels.Add(this.TrashMessagesViewModel);

            PageViewModels.Add(this.ReadMessageViewModel);
            PageViewModels.Add(this.ComposeMessageViewModel);

            // Set starting page
            this.ChangeViewModel(AuthorizationViewModel);
        }

        #region Properties / Commands

        public IPageViewModel AuthorizationViewModel
        {
            get;
            set;
        }

        public IPageViewModel RegistrationViewModel
        {
            get;
            set;
        }

        public IPageViewModel InboxMessagesViewModel
        {
            get;
            set;
        }

        public IPageViewModel SentMessagesViewModel
        {
            get;
            set;
        }

        public IPageViewModel TrashMessagesViewModel
        {
            get;
            set;
        }

        public IPageViewModel ReadMessageViewModel
        {
            get;
            set;
        }

        public IPageViewModel ComposeMessageViewModel
        {
            get;
            set;
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

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
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
                return _notificationBox;
            }
            set 
            {
                if (_notificationBox != value)
                {
                    _notificationBox = value;
                    OnPropertyChanged("NotificationBox");
                }
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);

            HideNotificationBox();

            viewModel.OnShow();
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

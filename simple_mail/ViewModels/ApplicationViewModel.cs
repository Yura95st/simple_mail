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

        #endregion

        public ApplicationViewModel()
        {
            RegisterToLogInUserMessage();
            RegisterToLogOutUserMessage();

            // list of available pages
            AuthorizationViewModel = new AuthorizationViewModel();
            RegistrationViewModel = new RegistrationViewModel();
            InboxMessagesViewModel = new InboxMessagesViewModel();
            SentMessagesViewModel = new SentMessagesViewModel();

            // Add available pages
            PageViewModels.Add(this.AuthorizationViewModel);
            PageViewModels.Add(this.RegistrationViewModel);
            PageViewModels.Add(this.InboxMessagesViewModel);
            PageViewModels.Add(this.SentMessagesViewModel);

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

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);

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

        #endregion
    }
}

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
        private BaseViewModel _currentPageViewModel;
        private List<BaseViewModel> _pageViewModels;

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
            CurrentPageViewModel = AuthorizationViewModel;
        }

        #region Properties / Commands

        public BaseViewModel AuthorizationViewModel
        {
            get;
            set;
        }

        public BaseViewModel RegistrationViewModel
        {
            get;
            set;
        }

        public BaseViewModel InboxMessagesViewModel
        {
            get;
            set;
        }

        public BaseViewModel SentMessagesViewModel
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
                        p => ChangeViewModel((BaseViewModel)p),
                        p => (p is BaseViewModel) && p != CurrentPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<BaseViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<BaseViewModel>();

                return _pageViewModels;
            }
        }

        public BaseViewModel CurrentPageViewModel
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

        private void ChangeViewModel(BaseViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void RegisterToLogInUserMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.UserLoggedIn, (Action)delegate() {
                    CurrentPageViewModel = InboxMessagesViewModel;
            });
        }

        private void RegisterToLogOutUserMessage()
        {
            ViewModelCommunication.Messaging.Register(ViewModelCommunication.UserLoggedOut, (Action)delegate()
            {
                CurrentPageViewModel = AuthorizationViewModel;
            });
        }

        #endregion
    }
}

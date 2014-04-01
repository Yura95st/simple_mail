using data_models.HelperClasses;
using data_models.Models;

namespace simple_mail.ViewModels
{
    /// <summary>
    /// Provides common functionality for ViewModel classes
    /// </summary>
    public abstract class BaseViewModel : ObservableObject
    {
        protected Notification _notification = Notification.Instance; 
    }
}

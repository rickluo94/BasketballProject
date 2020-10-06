using First_MVVM.Notifications;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace First_MVVM.ViewModels
{
    public class RegisterEmailViewModel : BindableBase, IInteractionRequestAware
    {
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand<object> CheckCommand { get; private set; }
        private string _emailStr;
        public string EmailStr
        {
            get { return _emailStr; }
            set { SetProperty(ref _emailStr, value); }
        }
        private string _noticeText;
                
        public string NoticeText
        {
            get { return _noticeText; }
            set { SetProperty(ref _noticeText, value); }
        }


        public RegisterEmailViewModel()
        {
            CancelCommand = new DelegateCommand(CancelInteraction);
            CheckCommand = new DelegateCommand<object>(CheckEmailStr);
        }
        private async void CheckEmailStr(object parameter)
        {
            var emailBox = parameter as TextBox;
            var returnedTaskTResult = IsValidEmail(emailBox.Text);
            bool boolResult = await returnedTaskTResult;
            if (boolResult)
            {
                NoticeText = "正確";
            }
            else
            {
                NoticeText = "有誤";
            }
        }
        
        private async Task<bool> IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private void CancelInteraction()
        {
            EmailStr = null;
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }
        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }
    }
}

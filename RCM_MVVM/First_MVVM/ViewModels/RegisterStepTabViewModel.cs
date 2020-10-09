using First_MVVM.Models;
using First_MVVM.Notifications;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using EasyCardModel;
using System.Windows.Navigation;
using DBModel;

namespace First_MVVM.ViewModels
{
    public class RegisterStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        //private ElectronicCoinModel _eCM = new ElectronicCoinModel();
        private RegisterModel _registerModel = new RegisterModel();
        private DB_Search _dB_Search = new DB_Search();
        private SendMessageModel _SMM = new SendMessageModel();
        private EasyCard easyCard = new EasyCard();
        private DB _dB = new DB();

        private int _selectedStepTabIndex = 0;
        public int SelectedStepTabIndex
        {
            get { return _selectedStepTabIndex; }
            set { SetProperty(ref _selectedStepTabIndex, value); }
        }

        private string _accountBuffer;
        private string _accountStr;
        public string AccountStr
        {
            get { return _accountStr; }
            set { SetProperty(ref _accountStr , value); }
        }

        private string _passwordStr;
        public string PasswordStr
        {
            get { return _passwordStr; }
            set { SetProperty(ref _passwordStr, value); }
        }

        private string _PasswordStrBuffer;
        public string PasswordStrBuffer
        {
            get { return _PasswordStrBuffer; }
            set { SetProperty(ref _PasswordStrBuffer, value); }
        }

        private string _emailStr;
        public string EmailStr
        {
            get { return _emailStr; }
            set { SetProperty(ref _emailStr, value); }
        }
        private string _cardID;

        public string CardID
        {
            get { return _cardID; }
            set { SetProperty(ref _cardID, value); }
        }


        private string _noticeText;
        public string NoticeText
        {
            get { return _noticeText; }
            set { SetProperty(ref _noticeText, value); }
        }


        public DelegateCommand<TextBox> AccountCmd { get; private set; }
        public DelegateCommand<TextBox> AccountLostFocusCmd { get; private set; }
        public DelegateCommand<TextBox> SMCmd { get; private set; }
        public DelegateCommand<TextBox> VerifySMCommand { get; private set; }
        public DelegateCommand<object> PasswordCommand { get; private set; }
        public DelegateCommand<object> PasswordConfirmCommand { get; private set; }
        public DelegateCommand<object> PasswordClearCommand { get; private set; }
        public DelegateCommand<TextBox> EmailCommand { get; private set; }
        public DelegateCommand ReadCardCommand { get; private set; }
        public DelegateCommand NextTabCommand { get; private set; }
        public DelegateCommand PreviousTabCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        

        public RegisterStepTabViewModel()
        {
            easyCard.SetDevicePort("COM4", 115200, 500); easyCard.Open();
            AccountCmd = new DelegateCommand<TextBox>(_checkAccount);
            AccountLostFocusCmd = new DelegateCommand<TextBox>(_isAccountExists);
            SMCmd = new DelegateCommand<TextBox>(SendMessageKey);
            VerifySMCommand = new DelegateCommand<TextBox>(VerifyMessageKey);
            PasswordCommand = new DelegateCommand<object>(CheckPassword);
            PasswordConfirmCommand = new DelegateCommand<object>(ConfirmPassword);
            PasswordClearCommand = new DelegateCommand<object>(PasswordClear);
            EmailCommand = new DelegateCommand<TextBox>(CheckEmailStr);
            ReadCardCommand = new DelegateCommand(ReadCard);
            NextTabCommand = new DelegateCommand(NextTab);
            PreviousTabCommand = new DelegateCommand(PreviousTab);
            ExitCommand = new DelegateCommand(ExitInteraction);
        }

        private async void _checkAccount(TextBox AccountBox)
        {
            if (string.IsNullOrWhiteSpace(AccountBox.Text) || _accountBuffer == AccountBox.Text) return;
            if (_registerModel.IsPhoneNumber(AccountBox.Text) && AccountBox.Text.Length == 10)
            {
                DataTable table = await _dB.Read(AccountBox.Text);
                if (table.Rows.Count > 0)
                {
                    NoticeText = "此帳號已存在";
                }
                else
                {
                    _accountBuffer = AccountBox.Text;
                    NoticeText = "可用";
                }
            }
            else
            {
                NoticeText = "不正確";
            }
        }

        private async void _isAccountExists(TextBox AccountBox)
        {
            
        }

        private void SendMessageKey(TextBox AccountBox)
        {
            _SMM.SmSendSampleCode(AccountBox.Text);
        }

        private void VerifyMessageKey(TextBox VerifySMBox)
        {
            _dB_Search.Verify_SmPhoneBinding_Confirm(AccountStr, VerifySMBox.Text);
        }


        private void CheckPassword(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (_registerModel.Checkpassword(passwordBox.Password))
            {
                PasswordStr = passwordBox.Password;
                if (PasswordStrBuffer == PasswordStr)
                {
                    NoticeText = "可用";
                }
                else
                {
                    NoticeText = "密碼不一致";
                }
            }
            else
            {
                passwordBox.Clear();
                NoticeText = string.Empty;
            }
        }

        private void ConfirmPassword(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (_registerModel.Checkpassword(passwordBox.Password))
            {
                PasswordStrBuffer = passwordBox.Password;
                if (passwordBox.Password == PasswordStr)
                {
                    NoticeText = "可用";
                }
                else
                {
                    NoticeText = "密碼不一致";
                }
            }
            else
            {
                passwordBox.Clear();
                NoticeText = string.Empty;
            }
        }

        private void PasswordClear(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            passwordBox.Clear();
        }

        private async void CheckEmailStr(TextBox emailBox)
        {
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

        private async void ReadCard()
        {
            CardID = "請靠感應";
            string Data = await Task.Run<string>(() => { return easyCard.Read_card_balance_request(); });
            CardID = (string)JObject.Parse(Data)["result"]["card_id"];
        }

        private void NextTab()
        {
            SelectedStepTabIndex += 1;
        }
        private void PreviousTab()
        {
            SelectedStepTabIndex -= 1;
        }
        private void ExitInteraction()
        {
            AccountStr = null;
            EmailStr = null;
            NoticeText = null;
            CardID = null;
            SelectedStepTabIndex = 0;
            FinishInteraction?.Invoke();
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

        public Action FinishInteraction { get; set; }

        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }
    }
}

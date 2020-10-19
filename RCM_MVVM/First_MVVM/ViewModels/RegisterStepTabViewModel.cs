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
using RfidModel;
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
        private EasyCard _easyCard = new EasyCard();
        private RFID _rfid = new RFID();
        private DB _dB = new DB();

        private string _rfidStr;

        public string RfidStr
        {
            get { return _rfidStr; }
            set { SetProperty(ref _rfidStr, value); }
        }


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

        private bool _nextStepIsEnabledBool;
        public bool NextStepIsEnabledBool
        {
            get { return _nextStepIsEnabledBool; }
            set { SetProperty(ref _nextStepIsEnabledBool, value); }
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


        //TEST
        public DelegateCommand OpenCmd { get; private set; }
        public DelegateCommand CloseCmd { get; private set; }

        public RegisterStepTabViewModel()
        {
            _easyCard.SetDevicePort("COM6", 115200, 500); _easyCard.Open();
            _rfid.SetDevicePort("COM4", 115200, 1000);
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

            //TEST
            OpenCmd = new DelegateCommand(RFIDOpen);
            CloseCmd = new DelegateCommand(RFIDClose);
        }

        private async void RFIDOpen()
        {
            bool Data = await Task.Run<bool>(() => { return _rfid.Open(); });
            if (Data == true)
            {
                RfidStr = "連線成功";
            }
            else
            {
                RfidStr = "連線失敗";
            }
        }
        private async void RFIDClose()
        {
            bool Data = await Task.Run<bool>(() => { return _rfid.Close(); });
            if (Data == true)
            {
                RfidStr = "斷線成功";
            }
            else
            {
                RfidStr = "斷線失敗";
            }
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
            bool _verify = _dB_Search.Verify_SmPhoneBinding_Confirm(AccountStr, VerifySMBox.Text);
            if (_verify == true)
            {
                NoticeText = "驗證成功";
                NextStepIsEnabledBool = true;
            }
            else
            {
                NoticeText = "驗證有誤";
                NextStepIsEnabledBool = false;
            }
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
                    NextStepIsEnabledBool = true;
                }
                else
                {
                    NoticeText = "密碼不一致";
                    NextStepIsEnabledBool = false;
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
                    NextStepIsEnabledBool = true;
                }
                else
                {
                    NoticeText = "密碼不一致";
                    NextStepIsEnabledBool = false;
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
                NextStepIsEnabledBool = true;
            }
            else
            {
                NoticeText = "有誤";
                NextStepIsEnabledBool = false;
            }
        }

        private async void ReadCard()
        {
            CardID = "請靠感應";
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            CardID = (string)JObject.Parse(Data)["result"]["card_id"];
        }

        private void NextTab()
        {
            SelectedStepTabIndex += 1;
            switch (_selectedStepTabIndex)
            {
                case 1:
                    _registerModel.ID = _accountStr;
                    break;
                case 2:
                    _registerModel.Password = _passwordStr;
                    break;
                case 3:
                    _registerModel.Email = _emailStr;
                    break;
                default:
                    break;
            }
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

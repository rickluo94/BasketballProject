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
using System.Diagnostics;

namespace First_MVVM.ViewModels
{
    public class RegisterStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        private RegisterModel _registerModel = new RegisterModel();
        private SendMessageModel _sendMessageModel = new SendMessageModel();
        private EasyCard _easyCard = new EasyCard();
        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();

        private int _selectedStepTabIndex;
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

        private string _nameStr;

        public string NameStr
        {
            get { return _nameStr; }
            set { SetProperty(ref _nameStr, value); }
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

        private bool _sMCmdIsEnabledBool;

        public bool SMCmdIsEnabledBool
        {
            get { return _sMCmdIsEnabledBool; }
            set { SetProperty(ref _sMCmdIsEnabledBool , value); }
        }

        private bool _accountBoxIsEnabled;

        public bool AccountBoxIsEnabled
        {
            get { return _accountBoxIsEnabled; }
            set { SetProperty(ref _accountBoxIsEnabled , value); }
        }


        public DelegateCommand RegisterStepTabLoadCmd { get; private set; }
        public DelegateCommand<TextBox> AccountCmd { get; private set; }
        public DelegateCommand<TextBox> SMCmd { get; private set; }
        public DelegateCommand<TextBox> VerifySMCommand { get; private set; }
        public DelegateCommand<object> PasswordCommand { get; private set; }
        public DelegateCommand<object> PasswordConfirmCommand { get; private set; }
        public DelegateCommand<object> PasswordClearCommand { get; private set; }
        public DelegateCommand<TextBox> NameCommand { get; private set; }
        public DelegateCommand<TextBox> EmailCommand { get; private set; }
        public DelegateCommand ReadCardCommand { get; private set; }
        public DelegateCommand NextTabCommand { get; private set; }
        public DelegateCommand PreviousTabCommand { get; private set; }
        public DelegateCommand RegisterSuccess { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
       
        public RegisterStepTabViewModel()
        {
            RegisterStepTabLoadCmd = new DelegateCommand(RegisterStepTabLoad);
            _easyCard.SetDevicePort("COM6", 115200, 500); _easyCard.Open();
            AccountCmd = new DelegateCommand<TextBox>(_checkAccount);
            SMCmd = new DelegateCommand<TextBox>(SendMessageKey);
            VerifySMCommand = new DelegateCommand<TextBox>(VerifyMessageKey);
            PasswordCommand = new DelegateCommand<object>(CheckPassword);
            PasswordConfirmCommand = new DelegateCommand<object>(ConfirmPassword);
            PasswordClearCommand = new DelegateCommand<object>(PasswordClear);
            NameCommand = new DelegateCommand<TextBox>(CheckNameStr);
            EmailCommand = new DelegateCommand<TextBox>(CheckEmailStr);
            ReadCardCommand = new DelegateCommand(ReadCard);
            NextTabCommand = new DelegateCommand(NextTab);
            PreviousTabCommand = new DelegateCommand(PreviousTab);
            RegisterSuccess = new DelegateCommand(RegisterAction);
            ExitCommand = new DelegateCommand(ExitInteraction);
        }

        private void RegisterStepTabLoad()
        {
            SelectedStepTabIndex = 0;
            AccountBoxIsEnabled = true;
            SMCmdIsEnabledBool = false;
            NextStepIsEnabledBool = false;
            _registerModel.ID = null;
            _registerModel.Password = null;
            _registerModel.Name = null;
            _registerModel.Email = null;
            _registerModel.Address = null;
            _registerModel.CardNumber = null;
            AccountStr = null;
            PasswordStr = null;
            PasswordStrBuffer = null;
            NameStr = null;
            EmailStr = null;
            CardID = null;
            NoticeText = string.Empty;
        }

        private async void _checkAccount(TextBox AccountBox)
        {
            if (string.IsNullOrWhiteSpace(AccountBox.Text)) 
            {
                NoticeText = "不可為空白";
                SMCmdIsEnabledBool = false;
                return;
            } 

            if (_registerModel.IsPhoneNumber(AccountBox.Text) && AccountBox.Text.Length == 10)
            {
                DataTable table = await _dBRead.Customer_Address(AccountBox.Text);
                if (table.Rows.Count > 0)
                {
                    NoticeText = "此帳號已存在";
                    SMCmdIsEnabledBool = false;
                }
                else
                {
                    _accountBuffer = AccountBox.Text;
                    NoticeText = "可用";
                    SMCmdIsEnabledBool = true;
                }
            }
            else
            {
                NoticeText = "不正確";
                SMCmdIsEnabledBool = false;
            }
        }

        private async void SendMessageKey(TextBox AccountBox)
        {
            string _phoneNumber = AccountBox.Text;
            string _randomKey = _sendMessageModel.RandomKey(6);

            #region 資料庫寫入true=>發送驗證碼

            bool result = await _dBWrite.Verify_SmPhoneBinding(_phoneNumber,_randomKey);
            if (result == true)
            {
                _sendMessageModel.SmSendSampleCode(_phoneNumber, _randomKey);
                AccountBoxIsEnabled = false;
                SMCmdIsEnabledBool = false;
            }
            else
            {
                bool resultAgain = await _dBWrite.Verify_SmPhoneBinding_UPDATE(_phoneNumber, _randomKey);
                if (resultAgain == true)
                {
                    _sendMessageModel.SmSendSampleCode(_phoneNumber, _randomKey);
                    AccountBoxIsEnabled = false;
                    SMCmdIsEnabledBool = false;
                }
            }
            #endregion

        }

        private async void VerifyMessageKey(TextBox VerifySMBox)
        {
            DataTable table = await _dBRead.Verify_SmPhoneBinding(AccountStr, VerifySMBox.Text);
            if (table.Rows.Count > 0)
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

        private async void CheckNameStr(TextBox nameBox)
        {
            _registerModel.Name = nameBox.Text;
            bool boolResult = await Task.Run<bool>(() => { return _registerModel.IsChinese(_registerModel.Name); });
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

        private async void CheckEmailStr(TextBox emailBox)
        {
            _registerModel.Email = emailBox.Text;
            bool boolResult = await Task.Run<bool>(() => { return _registerModel.IsValidEmail(_registerModel.Email); });
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
            NextStepIsEnabledBool = false;
            NoticeText = null;
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
                    _registerModel.Name = _nameStr;
                    break;
                case 4:
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

        private async void RegisterAction()
        {
            bool RegisterAccount = await _dBWrite.Customer_info(_registerModel.ID, _registerModel.Name, _registerModel.Email);

            if (RegisterAccount == true)
            {
                ExitInteraction();
            }
        }

        private void ExitInteraction()
        {
            _registerModel.ID = null;
            _registerModel.Password = null;
            _registerModel.Name = null;
            _registerModel.Email = null;
            _registerModel.Address = null;
            _registerModel.CardNumber = null;
            AccountStr = null;
            PasswordStr = null;
            PasswordStrBuffer = null;
            NameStr = null;
            EmailStr = null;
            CardID = null;
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

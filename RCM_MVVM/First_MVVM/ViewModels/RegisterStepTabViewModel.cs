using First_MVVM.Models;
using First_MVVM.Business;
using First_MVVM.Notifications;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using EasyCardModel;
using DBModel;
using SendMessageModel;
using System.Timers;
using System.Collections.Generic;

namespace First_MVVM.ViewModels
{
    public class RegisterStepTabViewModel : BindableBase, IInteractionRequestAware
    {
        private RegisterModel _registerModel { get; set; }
        private NationalCities _nationalCities = new NationalCities();
        private StrVerify _strVerify = new StrVerify();
        private SendMessage _sendMessage = new SendMessage();
        private EasyCard _easyCard = new EasyCard();
        private EasyCardServ _easyCardServ = new EasyCardServ();
        private DBRead _dBRead = new DBRead();
        private DBWrite _dBWrite = new DBWrite();

        #region Interface Preperty
        private bool _readRulesIsChecked;
        public bool ReadRulesIsChecked
        {
            get { return _readRulesIsChecked; }
            set { SetProperty(ref _readRulesIsChecked, value); }
        }

        private int _selectedStepTabIndex;
        public int SelectedStepTabIndex
        {
            get { return _selectedStepTabIndex; }
            set { SetProperty(ref _selectedStepTabIndex, value); }
        }

        private string _selectedStepTabName;
        public string SelectedStepTabName
        {
            get { return _selectedStepTabName; }
            set { SetProperty(ref _selectedStepTabName, value); }
        }

        private int _selectedCitiesIndex;

        public int SelectedCitiesIndex
        {
            get { return _selectedCitiesIndex; }
            set { SetProperty(ref _selectedCitiesIndex, value); }
        }

        private string _selectedCities;

        public string SelectedCities
        {
            get { return _selectedCities; }
            set { SetProperty(ref _selectedCities, value); }
        }

        private string _selectedTownship;

        public string SelectedTownship
        {
            get { return _selectedTownship; }
            set { SetProperty(ref _selectedTownship, value); }
        }

        private List<string> _cities;

        public List<string> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        private List<string> _township;

        public List<string> Township
        {
            get { return _township; }
            set { SetProperty(ref _township, value); }
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

        private string _card_ID;
        public string Card_ID
        {
            get { return _card_ID; }
            set { SetProperty(ref _card_ID, value); }
        }

        private string _card_purse_id;
        public string Card_purse_id
        {
            get { return _card_purse_id; }
            set { SetProperty(ref _card_purse_id, value); }
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
        #endregion 資料流 資料

        #region Interface Command
        public DelegateCommand CheckReadRulesCmd { get; private set; }
        public DelegateCommand RegisterStepTabLoadCmd { get; private set; }
        public DelegateCommand<TextBox> AccountCmd { get; private set; }
        public DelegateCommand<TextBox> SMCmd { get; private set; }
        public DelegateCommand<TextBox> VerifySMCmd { get; private set; }
        public DelegateCommand<object> PasswordCmd { get; private set; }
        public DelegateCommand<object> PasswordConfirmCmd { get; private set; }
        public DelegateCommand<object> PasswordClearCmd { get; private set; }
        public DelegateCommand<TextBox> NameCmd { get; private set; }

        public DelegateCommand CitiesIsChangeCmd { get; private set; }
        public DelegateCommand TownshipIsChangeCmd { get; private set; }

        public DelegateCommand<TextBox> EmailCmd { get; private set; }
        public DelegateCommand ReadCardCmd { get; private set; }
        public DelegateCommand NextTabCmd { get; private set; }
        public DelegateCommand PreviousTabCmd { get; private set; }
        public DelegateCommand RegisterSuccessCmd { get; private set; }
        public DelegateCommand ExitCmd { get; private set; }
        #endregion

        public RegisterStepTabViewModel()
        {
            _registerModel = new RegisterModel();
            CheckReadRulesCmd = new DelegateCommand(CheckReadRules);
            RegisterStepTabLoadCmd = new DelegateCommand(RegisterStepTabLoad);
            AccountCmd = new DelegateCommand<TextBox>(_checkAccount);
            SMCmd = new DelegateCommand<TextBox>(SendMessageKey);
            VerifySMCmd = new DelegateCommand<TextBox>(VerifyMessageKey);
            PasswordCmd = new DelegateCommand<object>(CheckPassword);
            PasswordConfirmCmd = new DelegateCommand<object>(ConfirmPassword);
            PasswordClearCmd = new DelegateCommand<object>(PasswordClear);
            NameCmd = new DelegateCommand<TextBox>(CheckNameStr);
            CitiesIsChangeCmd = new DelegateCommand(FillTownshipData);
            TownshipIsChangeCmd = new DelegateCommand(CheckAddress);
            EmailCmd = new DelegateCommand<TextBox>(CheckEmailStr);
            ReadCardCmd = new DelegateCommand(ReadCard);
            NextTabCmd = new DelegateCommand(NextTab);
            PreviousTabCmd = new DelegateCommand(PreviousTab);
            RegisterSuccessCmd = new DelegateCommand(RegisterAction);
            ExitCmd = new DelegateCommand(ExitInteraction);
            Cities = _nationalCities.FillCitiesData(_nationalCities.FillDefaultData());
        }

        private void CheckReadRules()
        {
            if (_readRulesIsChecked == true)
            {
                NextStepIsEnabledBool = true;
            }
            else
            {
                NextStepIsEnabledBool = false;
            }
        }

        private void RegisterStepTabLoad()
        {
            _easyCard.SetDevicePort("COM7", 115200, 500); _easyCard.Open();
            ReadRulesIsChecked = false;
            SelectedCitiesIndex = -1;
            SelectedStepTabIndex = 0;
            AccountBoxIsEnabled = true;
            SMCmdIsEnabledBool = false;
            NextStepIsEnabledBool = false;
            AccountStr = null;
            PasswordStr = null;
            PasswordStrBuffer = null;
            NameStr = null;
            EmailStr = null;
            Card_ID = null;
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

            if (_strVerify.IsPhoneNumber(AccountBox.Text) && AccountBox.Text.Length == 10)
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
            string _randomKey = _sendMessage.RandomKey(6);

            #region 資料庫寫入true=>發送驗證碼

            bool result = await _dBWrite.Verify_SmPhoneBinding(_phoneNumber,_randomKey);
            if (result == true)
            {
                _sendMessage.SmSendSampleCode(_phoneNumber, _randomKey);
                AccountBoxIsEnabled = false;
                SMCmdIsEnabledBool = false;
            }
            else
            {
                bool resultAgain = await _dBWrite.Verify_SmPhoneBinding_UPDATE(_phoneNumber, _randomKey);
                if (resultAgain == true)
                {
                    _sendMessage.SmSendSampleCode(_phoneNumber, _randomKey);
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
            if (_strVerify.Checkpassword(passwordBox.Password))
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
            if (_strVerify.Checkpassword(passwordBox.Password))
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
            bool boolResult = await Task.Run<bool>(() => { return _strVerify.IsChinese(_registerModel.Name); });
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

        private void FillTownshipData()
        {
            Township = _nationalCities.FillTownshipData(_nationalCities.FillDefaultData(), _selectedCitiesIndex);
        }

        private void CheckAddress()
        {
            if (!string.IsNullOrWhiteSpace(_selectedCities) && !string.IsNullOrWhiteSpace(_selectedTownship))
            {
                NextStepIsEnabledBool = true;
            }
            else
            {
                NextStepIsEnabledBool = false;
            }
        }

        private async void CheckEmailStr(TextBox emailBox)
        {
            _registerModel.Email = emailBox.Text;
            bool boolResult = await Task.Run<bool>(() => { return _strVerify.IsValidEmail(_registerModel.Email); });
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
            Card_ID = "請靠感應";
            string Data = await Task.Run<string>(() => { return _easyCard.Read_card_balance_request(); });
            Card_ID = (string)JObject.Parse(Data)["result"]["card_id"];
            Card_purse_id = (string)JObject.Parse(Data)["result"]["card_purse_id"];
            bool isThisAlreadyHadBinding = _easyCardServ.IsThisAlreadyHadBinding(Card_ID);

            //Card_ID = "4334488813";
            //Card_purse_id = "4334488813";
            if (!string.IsNullOrWhiteSpace(Card_ID))
            {
                if (isThisAlreadyHadBinding == true)
                {
                    NoticeText = "悠遊卡不可重複綁定";
                    NextStepIsEnabledBool = false;
                }
                else
                {
                    NextStepIsEnabledBool = true;
                }
            }
            else
            {
                NextStepIsEnabledBool = false;
            }
           
        }

        private void FillProfile()
        {
            switch (_selectedStepTabName)
            {
                case "帳號":
                    _registerModel.ID = _accountStr;
                    break;
                case "密碼":
                    _registerModel.Password = _passwordStr;
                    break;
                case "個人資料":
                    _registerModel.Name = _nameStr;
                    break;
                case "地區":
                    _registerModel.City = _selectedCities;
                    _registerModel.Area = _selectedTownship;
                    break;
                case "信箱":
                    _registerModel.Email = _emailStr;
                    break;
                case "設定悠遊卡":
                    _registerModel.Card_id = _card_ID;
                    _registerModel.Card_purse_id = _card_purse_id;
                    break;
                default:
                    break;
            }
        }

        private void NextTab()
        {
            FillProfile();
            NextStepIsEnabledBool = false;
            NoticeText = null;
            SelectedStepTabIndex += 1;
        }

        private void PreviousTab()
        {
            SelectedStepTabIndex -= 1;
        }

        private async void RegisterAction()
        {
            bool RegisterAccount = await _dBWrite.Customer_info(_registerModel.ID, _registerModel.Name, _registerModel.Email);
            bool RegisterLogin = await _dBWrite.Users(_registerModel.ID, _registerModel.Name, _registerModel.Password);
            bool RegisterAddress = await _dBWrite.Customer_Address(_registerModel.ID, _registerModel.City, _registerModel.Area);
            bool RegisterCardPofile = await _dBWrite.RFID_Users(_registerModel.ID, _registerModel.Card_id, _registerModel.Card_purse_id);
            if (RegisterAccount == RegisterLogin == RegisterAddress == RegisterCardPofile == true)
            {
                ExitInteraction();
            }
        }

        private void ExitInteraction()
        {
            AccountStr = null;
            PasswordStr = null;
            PasswordStrBuffer = null;
            NameStr = null;
            EmailStr = null;
            Card_ID = null;
            _easyCard.Close();
            FinishInteraction?.Invoke();
        }

        #region MainWindow Interactive

        public Action FinishInteraction { get; set; }

        private ICustomNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (ICustomNotification)value); }
        }

        #endregion

    }
}

using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Controls;
using First_MVVM.Models;
using System.Data;

namespace First_MVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        DB_Search dB_Search = new DB_Search();

        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _passWord;

        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }



        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }


        public DelegateCommand<object> LoginCommand { get; private set; }

        public MainWindowViewModel()
        {
            LoginCommand = new DelegateCommand<object>(LoginExecute);
        }

        public void LoginExecute(object parameter) 
        {
            var passwordBox = parameter as PasswordBox;

            if (passwordBox != null) { PassWord = passwordBox.Password; }

            if (!String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(PassWord))
            {
                DataTable _userInfo = dB_Search.User_Info(UserName);

                if (_userInfo.Rows[0]["password"].ToString() == PassWord)
                {
                    UpdateText = "登入成功";
                }
                else
                {
                    UpdateText = "登入失敗";
                }
            }
            else
            {
                UpdateText = "帳號密碼不可為空白";
            }
        }
    }
}

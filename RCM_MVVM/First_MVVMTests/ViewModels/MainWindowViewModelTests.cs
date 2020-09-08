using Microsoft.VisualStudio.TestTools.UnitTesting;
using First_MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Windows.Threading;

namespace First_MVVM.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        [TestMethod()]
        public void LoginExecuteTest()
        {
            var Sut = new MainWindowViewModel();
            Sut.PassWord = "123456";
            Sut.UserName = "root";
            var expected = "登入成功";

            Sut.LoginExecute(null);
            //act
            var actual = Sut.UpdateText;

            //assert
            Assert.AreEqual(expected, actual);

        }
    }
}
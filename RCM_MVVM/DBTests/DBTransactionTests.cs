using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBModel.Tests
{
    [TestClass()]
    public class DBTransactionTests
    {
        [TestMethod()]
        public void RegisterAccountTest正常註冊()
        {
            bool expected = true;
            string ID = "0927818696";
            string Email = "edcrfvlily5@gmail.com";
            string Password = "123456";
            string City = "台中";
            string Area = "西屯";
            string Card_id = "42876046";
            string Card_purse_id = "42876046";

            var Sut = new DBTransaction();

            var actual = Sut.RegisterAccount(ID, Email, Password, City, Area, Card_id, Card_purse_id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RegisterAccountTest異常資料寫入()
        {
            bool expected = false;
            string ID = "0927818696";
            string Email = "edcrfvlily5@gmail.com";
            string Password = "123456";
            string City = "台中台中台中台中台中台中台中台中";
            string Area = "西屯西屯西屯西屯西屯西屯西屯西屯";
            string Card_id = "42876046";
            string Card_purse_id = "00000000";

            var Sut = new DBTransaction();

            var actual = Sut.RegisterAccount(ID, Email, Password, City, Area, Card_id, Card_purse_id);

            Assert.AreEqual(expected, actual);
        }

    }
}
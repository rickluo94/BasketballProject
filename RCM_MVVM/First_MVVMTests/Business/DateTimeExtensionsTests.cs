using Microsoft.VisualStudio.TestTools.UnitTesting;
using First_MVVM.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Business.Tests
{
    [TestClass()]
    public class DateTimeExtensionsTests
    {
        [TestMethod()]
        public void 得到七天前日期()
        {
            DateTime date = new DateTime(2020,12,20);
            DateTime expected = new DateTime(2020,12,13);
            var Sut = new DateTimeExtensions();

            var actual = Sut.GetDateOfLastDays(date, -7);

            Assert.AreEqual(actual, expected);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using First_MVVM.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Business.Tests
{
    [TestClass()]
    public class PaymentTests
    {
        [TestMethod()]
        public void 使用30分鐘_點數0()
        {
            int Amount = 15;
            int Surplus = 0;
            var Sut = new Payment();
            
            var actual = Sut.CountAmount(30,0);

            Assert.AreEqual(Amount, actual.Amount);
            Assert.AreEqual(Surplus, actual.Surplus);
        }

        [TestMethod()]
        public void 使用30分鐘_點數1()
        {
            int Amount = 15;
            int Surplus = 1;
            var Sut = new Payment();

            var actual = Sut.CountAmount(30, 1);

            Assert.AreEqual(Amount, actual.Amount);
            Assert.AreEqual(Surplus, actual.Surplus);
        }

        [TestMethod()]
        public void 使用35分鐘_點數50()
        {
            int Amount = 15;
            int Surplus = 45;
            var Sut = new Payment();

            var actual = Sut.CountAmount(35, 50);

            Assert.AreEqual(Amount, actual.Amount);
            Assert.AreEqual(Surplus, actual.Surplus);
        }

        [TestMethod()]
        public void 使用90分鐘_點數50()
        {
            int Amount = 30;
            int Surplus = 20;
            var Sut = new Payment();

            var actual = Sut.CountAmount(90, 50);

            Assert.AreEqual(Amount, actual.Amount);
            Assert.AreEqual(Surplus, actual.Surplus);
        }

        [TestMethod()]
        public void 使用0分鐘_點數50()
        {
            int Amount = 0;
            int Surplus = 50;
            var Sut = new Payment();

            var actual = Sut.CountAmount(0, 50);

            Assert.AreEqual(Amount, actual.Amount);
            Assert.AreEqual(Surplus, actual.Surplus);
        }

    }
}
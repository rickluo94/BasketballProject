using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Business
{
    public class Payment
    {
        enum UnitCharge
        {
            Coin = 15,
        }

        enum UnitTime
        {
            Minute = 30,
        }

        /// <summary>
        /// return UseCoin and Amount and surplus
        /// </summary>
        /// <param name="UsageTime"></param>
        /// <param name="surplus"></param>
        /// <returns></returns>
        public (int Amount,int Surplus)CountAmount(int UsageTime,int Surplus)
        {
            int Amount = 0;

            if (UsageTime > 0 && UsageTime <= 30)
            {
                UsageTime -= (int)UnitTime.Minute;
                Amount = (int)UnitCharge.Coin;
                Surplus += Math.Abs(UsageTime);
                return (Amount: Amount, Surplus: Surplus);
            }
            else if (UsageTime > 30 && Surplus > 0)
            {
                //基本費扣除
                UsageTime -= (int)UnitTime.Minute;
                Amount += (int)UnitCharge.Coin;
                //扣除點數
                UsageTime -= Surplus;

                if (UsageTime < 0)
                {
                    Surplus = Math.Abs(UsageTime);
                    return (Amount: Amount, Surplus: Surplus);
                }
                else
                {
                    Surplus = 0;
                }
            }

            //扣除基本費及點數後累計
            //不足30分鐘以30分鐘計算，將剩餘點數回傳
            while (UsageTime > 0)
            {
                UsageTime -= (int)UnitTime.Minute;
                Amount += (int)UnitCharge.Coin;

                if (UsageTime < 0)
                {
                    Surplus += Math.Abs(UsageTime);
                }
            }

            return (Amount: Amount,Surplus: Surplus);
        }
    }

}

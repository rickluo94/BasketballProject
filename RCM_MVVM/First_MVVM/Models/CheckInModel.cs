using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    public class CheckInModel
    {
        public string ID { get; set; }
        public string CardID { get; set; }
        public string Balance { get; set; }
        public string Epc { get; set; }
        public string OutTime { get; set; }
        public string InTime { get; set; }
        public string DebitStatus { get; set; }
        public int Amount { get; set; }
        public string HoursUse { get; set; }
        public string LockerSelectedIndex { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    public class CheckInModel
    {
        public string SN { get; set; }
        public string ID { get; set; }
        public string CardID { get; set; }
        public string Balance { get; set; }
        public string EPC { get; set; }
        public string OutTime { get; set; }
        public string InTime { get; set; }
        public string DebitStatus { get; set; }
        public int Amount { get; set; }
        public string HoursUse { get; set; }
        public string LockerBoxSelectedIndex { get; set; }
    }
}

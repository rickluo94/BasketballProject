using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    public class CheckInModel
    {
        public string SN { get; set; }
        public string Take_SN { get; set; }
        public string ID { get; set; }
        public string CardID { get; set; }
        public string Balance { get; set; }
        public string EPC { get; set; }
        public DateTime OutTime { get; set; }
        public DateTime InTime { get; set; }
        public string DebitStatus { get; set; }
        public int Amount { get; set; }
        public int UsageTime { get; set; }
        public string LockerBoxSelectedIndex { get; set; }
    }
}

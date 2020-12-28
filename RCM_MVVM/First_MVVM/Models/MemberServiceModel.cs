using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    public class MemberServiceModel
    {
        public string PumpSN { get; set; }
        public string SN { get; set; }
        public string ID { get; set; }
        public string CardID { get; set; }
        public string Balance { get; set; }
        public int Amount { get; set; }
        public string DebitStatus { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime CheckIn { get; set; }
        public int TimePoint { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    public class MemberServiceModel
    {
        public string ID { get; set; }
        public string CardID { get; set; }
        public string Balance { get; set; }
        public int Amount { get; set; }
        public string DebitStatus { get; set; }
    }
}
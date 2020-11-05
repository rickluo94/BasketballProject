using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    public class RFID_ReaderModel
    {
        public bool Status { get; set; }
        public string EPC { get; set; }
        public string TID { get; set; }
    }
}

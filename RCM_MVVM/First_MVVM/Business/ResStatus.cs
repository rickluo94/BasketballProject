using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Business
{
    public class ResStatus
    {
        public bool A1 { get; private set; }
        public bool A2 { get; private set; }
        public bool A3 { get; private set; }
        public bool A4 { get; private set; }
        public bool A5 { get; private set; }
        public bool A6 { get; private set; }
        public bool A7 { get; private set; }

        public bool Update()
        {
            try
            {
                A1 = true;
                A2 = true;
                A3 = true;
                A4 = true;
                A5 = true;
                A6 = true;
                A7 = true;
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

    }
}

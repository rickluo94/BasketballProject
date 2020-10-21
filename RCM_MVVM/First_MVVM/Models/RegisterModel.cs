using System;
using System.Text.RegularExpressions;

namespace First_MVVM.Models
{
    public class RegisterModel
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CardNumber { get; set; }
        
        public bool AddNewCustomer()
        {

        }


        public bool Checkpassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            return Regex.IsMatch(password, "^[a-zA-Z0-9]*$");
        }

        /// <summary>
        /// 指示 Unicode 字元是否分類為十進位數字。
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public bool IsPhoneNumber(string Number)
        {
            if (String.IsNullOrWhiteSpace(Number))
                return false;
            foreach (char c in Number)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

    }
}

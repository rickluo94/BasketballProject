using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace First_MVVM.Models
{
    public class RegisterModel
    {
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

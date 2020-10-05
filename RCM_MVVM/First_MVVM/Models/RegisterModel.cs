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
        
    }
}

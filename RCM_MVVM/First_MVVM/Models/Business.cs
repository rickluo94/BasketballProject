using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace First_MVVM.Models
{
    public class Business
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

        /// <summary>
        /// 判斷字串文字是否皆為中文
        /// </summary>
        /// <param name="strChinese">中文字串</param>
        /// <returns>若字串皆為中文 :true 含有中文以外的文字 :false</returns>
        public bool IsChinese(string strChinese)
        {
            if (string.IsNullOrWhiteSpace(strChinese)) return false;
            int dRange = 0;
            int dstringmax = Convert.ToInt32("9fff", 16);
            int dstringmin = Convert.ToInt32("4e00", 16);
            bool flog = false;
            for (int i = 0; i < strChinese.Length; i++)
            {
                dRange = Convert.ToInt32(Convert.ToChar(strChinese.Substring(i, 1)));
                if (dRange >= dstringmin && dRange < dstringmax)
                {
                    flog = true;
                }
                else
                {
                    flog = false;
                }
            }
            return flog;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace First_MVVM.Models
{
    class DB_Write
    {
        DBModel dBModel = new DBModel();

        /// <summary>
        /// 手機簡訊驗證碼綁定號碼(寫入)
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <param name="RandomKey"></param>
        public void Verify_SmPhoneBinding(string PhoneNumber, string RandomKey)
        {
            dBModel.DatabaseCommand("ste_SBSCS", "INSERT INTO ste_SBSCS.Verify_SmPhoneBinding (Verify_user_id, Verify_SmKeyBinding) " +
                "VALUES ('" + PhoneNumber + "',SHA2('" + RandomKey + "',256)) ON DUPLICATE KEY UPDATE Verify_SmKeyBinding = SHA2('" + RandomKey + "',256);");
        }
    }
}

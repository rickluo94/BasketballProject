using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using DBModel;

namespace First_MVVM.Business
{
    public class AlreadyBox
    {
        private DBRead dBRead = new DBRead();
        private DataTable invenotryX { get; set; }
        Dictionary<string, bool> Status = new Dictionary<string, bool>();

        enum BoxName
        {
            A1,
            A2,
            A3,
            A4,
            A5,
            A6,
            A7
        }

        public async Task<Dictionary<string, bool>> UpdataStatus()
        {
            try
            {
                Status.Clear();
                invenotryX = await dBRead.InventoryX();

                foreach (string item in Enum.GetNames(typeof(BoxName)))
                {
                    int Boxfilter = invenotryX.Select("Inventory_CabinetLoc ='" + item + "' and Inventory_Amount = '1'").Length;
                    if (Boxfilter >= 1)
                    {
                        Status.Add(item, true);
                    }
                    else
                    {
                        Status.Add(item, false);
                    }
                }
                return Status;
            }
            catch (Exception)
            {
                foreach (string item in Enum.GetNames(typeof(BoxName)))
                {
                    Status.Add(item, false);
                }
                return Status;
            }
        }
    }
}

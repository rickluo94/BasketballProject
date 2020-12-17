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
        private LC_DBRead _lC_DBRead = new LC_DBRead();
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
                invenotryX = await _lC_DBRead.InventoryX();

                foreach (string item in Enum.GetNames(typeof(BoxName)))
                {
                    int Boxfilter = invenotryX.Select("CabinetLoc ='" + item + "' and Amount = '1'").Length;
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

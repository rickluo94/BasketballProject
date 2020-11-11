using DBModel;
using Renci.SshNet.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using First_MVVM.Models;

namespace First_MVVM.Business
{
    public interface BoxItemStatus
    {
        Task<List<bool>> GetAll();
    }

	public class DBBoxItemStatus : BoxItemStatus
	{
        AlreadyBox Box = new AlreadyBox();

        public async Task<List<bool>> GetAll()
		{
            List<bool> boolist = new List<bool>();
            Dictionary<string, bool> keyValuePairs = await Box.UpdataStatus();

            keyValuePairs.TryGetValue("A1",out bool A1);
            keyValuePairs.TryGetValue("A2",out bool A2);
            keyValuePairs.TryGetValue("A3",out bool A3);
            keyValuePairs.TryGetValue("A4",out bool A4);
            keyValuePairs.TryGetValue("A5",out bool A5);
            keyValuePairs.TryGetValue("A6",out bool A6);
            keyValuePairs.TryGetValue("A7",out bool A7);

            boolist.Add(A1);
            boolist.Add(A2);
            boolist.Add(A3);
            boolist.Add(A4);
            boolist.Add(A5);
            boolist.Add(A6);
            boolist.Add(A7);

            return boolist;
        }
	}
}

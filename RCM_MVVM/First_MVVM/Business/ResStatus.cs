using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace First_MVVM.Business
{
    public interface ResStatus
    {
        List<bool> GetAll();
    }

	public class DBResStatus : ResStatus
	{

        public List<bool> GetAll()
		{
            return new List<bool>()
			{
                true,
                false,
                false,
                false,
                false,
                false,
                false
            };
		}
	}
}

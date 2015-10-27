using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Data
{
    public sealed class DataProvider
    {
        private DataProvider() { }

        public static IDao Instance
        {
            get
            {
                return SqlDao.Instance;
            }
        }

    }
}
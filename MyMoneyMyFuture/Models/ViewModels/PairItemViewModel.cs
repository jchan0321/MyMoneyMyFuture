using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.ViewModels
{
    public class PairItemViewModel<T1, T2> : BaseViewModel
    {
        public T1 PrimaryItem { get; set; }
        public T2 SecondaryItem { get; set; }

    }
}
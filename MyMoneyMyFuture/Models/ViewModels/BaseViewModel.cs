using MyMoneyMyFuture.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.ViewModels
{
    public class BaseViewModel
    {// make note that this base class does not have to be, or should not be abstract. 
     // it is a perfectly good class to be used as a ViewModel 

        public bool IsLoggedIn { get; set; }
        //public AnalyticsCategory Category { get; set; }
        //public ViewType PageType { get; set; }
        //public AnalyticsAction Action { get; set; }
        public object AnalyticsData { get; internal set; }
        public bool IsAdmin { get; set; }
        public string FileDomainName { get; set; }
        public object LoadingInformation { get; set; }
        public List<PageMetaTagValue> PageMetaTag { get; set; }
    }
}
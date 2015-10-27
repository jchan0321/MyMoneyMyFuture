using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Domain
{
    public class PageMetaTagValue: MetaTag
    {
        public int PageId { get; set; }
        public string Value { get; set; }
    }
}
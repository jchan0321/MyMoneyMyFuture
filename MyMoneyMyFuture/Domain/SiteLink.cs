using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Domain
{
    public class SiteLink
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Type { get; set; }
        public int Group { get; set; }
        public int OwnerId { get; set; }
        public int OwnerType { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public string LanguageCode { get; set; }
    }
}
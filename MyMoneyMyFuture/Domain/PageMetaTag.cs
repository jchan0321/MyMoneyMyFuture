using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Domain
{
    public class PageMetaTag
    {
        public int Id { get; set; }
        public int MetaTagId { get; set; }
        public string Value { get; set; }
        public int OwnerId { get; set; }
        public int OwnerType { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public string LanguageCode { get; set; }
    }
}
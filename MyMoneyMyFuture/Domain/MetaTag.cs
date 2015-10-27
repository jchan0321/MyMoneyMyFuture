using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Domain
{
    public class MetaTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
        public string Template { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public string LanguageCode { get; set; }
    }
}
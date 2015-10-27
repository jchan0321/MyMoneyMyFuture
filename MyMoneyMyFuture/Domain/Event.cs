using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Domain
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventTime { get; set; }
        public string VenueName { get; set; }
        public Uri Url { get; set; } //data type uri
        public string Type { get; set; }
        public string Tags { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        //public string UserId { get; set; } // do not bring back
        public string LanguageCode { get; set; }
    }
}
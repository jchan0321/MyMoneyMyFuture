using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.Requests
{
    public class SiteLinksAddRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Url { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int Group { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public int OwnerType { get; set; }
    }
}
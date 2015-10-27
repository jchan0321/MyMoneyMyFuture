using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.Requests
{
    public class PageMetaTagsAddRequest
    {
        [Required]
        public int MetaTagId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Value { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public int OwnerType { get; set; }
    }
}
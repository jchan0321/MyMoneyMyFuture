using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.Requests
{
    public class PageMetaTagsUpdateRequest: PageMetaTagsAddRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
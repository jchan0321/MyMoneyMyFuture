using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.Requests
{
    public class SiteLinksUpdateRequest: SiteLinksAddRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
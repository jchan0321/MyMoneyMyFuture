using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.Response
{
    public class SuccessResponse : BaseResponse
    {
        public SuccessResponse()
        {
            this.IsSuccessFull = true;
        }
    }
}
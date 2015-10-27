using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMoneyMyFuture.Controllers
{
    public class SiteLinksController : BaseController
    {
        public override ViewType ViewType
        {
            get
            {
                return ViewType.SiteLinks;
            }
        }

        public SiteLinksController(IUserService uService)
            : base(uService)
        {

        }

        [Route("{ownerId:int}/{ownerType:int}")]
        public ActionResult SiteLinks(int ownerId, int ownerType)
        {
            PairItemViewModel<int, int> model = new PairItemViewModel<int, int>();
            model.PrimaryItem = ownerId;
            model.SecondaryItem = ownerType;

            return View(model);
        }
    }
}
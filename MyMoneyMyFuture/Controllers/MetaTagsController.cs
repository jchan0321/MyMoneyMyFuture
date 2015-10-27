using MyMoneyMyFuture.Models.ViewModels;
using MyMoneyMyFuture.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMoneyMyFuture.Controllers
{
    public class MetaTagsController : Controller
    {
        public override ViewType ViewType
        {
            get
            {
                return ViewType.MetaTags;
            }
        }

        public MetaTagsController(IUserService uService)
            : base(uService)
        {

        }

        // GET: MetaTags
        [Route("{ownerId:int}/{ownerType:int}")]
        public ActionResult Edit(int ownerId, int ownerType)
        {
            PairItemViewModel<int, int> model = new PairItemViewModel<int, int>();
            model.PrimaryItem = ownerId;
            model.SecondaryItem = ownerType;

            return View(model);
        }
    }
}
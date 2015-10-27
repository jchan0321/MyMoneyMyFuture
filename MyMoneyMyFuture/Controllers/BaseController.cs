using MyMoneyMyFuture.Models.ViewModels;
using MyMoneyMyFuture.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MyMoneyMyFuture.Controllers
{
    public abstract class BaseController : Controller
    {
        public abstract ViewType ViewType { get; }

        private IUserService _userService;
        private string _superAdmin;
        private string _admin;

        protected BaseController(IUserService userService)
        {
            this._userService = userService;
            this._superAdmin = WebConfigurationManager.AppSettings["superAdmin"];
            this._admin = WebConfigurationManager.AppSettings["admin"];
        }

        public virtual object GetAnalyticsData()
        {
            return null;
        }
    }
}
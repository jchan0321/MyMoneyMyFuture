using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models.Requests;
using MyMoneyMyFuture.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyMoneyMyFuture.Controllers.Api
{
    public class SiteLinksApiController : ApiController
    {
        private ISiteLinksService _siteLinksService;
        private IUserService _userService;

        public SiteLinksApiController(IUserService service, ISiteLinksService slservice)
        {
            this._userService = service;
            this._siteLinksService = slservice;
        }

        [Route, HttpPost]
        public HttpResponseMessage Add(SiteLinksAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string userId = _userService.GetCurrentUserId();
            ItemResponse<Int32> response = new ItemResponse<Int32>();

            response.Item = _siteLinksService.Add(model, userId);

            return Request.CreateResponse(response);
        }

        [Route("{siteLinkId:int}"), HttpPut]
        public HttpResponseMessage Update(SiteLinksUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse response = new SuccessResponse();

            _siteLinksService.Update(model);

            return Request.CreateResponse(response);
        }

        [AllowAnonymous]
        [Route("{ownerId:int}/{ownerType:int}"), HttpGet]
        public HttpResponseMessage GetByOwner(int ownerId, int ownerType)
        {
            ItemsResponse<SiteLink> response = new ItemsResponse<SiteLink>();

            response.Items = _siteLinksService.GetByOwner(ownerId, ownerType);

            return Request.CreateResponse(response);
        }

        [Route("{siteLinkId:int}"), HttpGet]
        public HttpResponseMessage GetById(int siteLinkId)
        {
            ItemResponse<SiteLink> response = new ItemResponse<SiteLink>();

            response.Item = _siteLinksService.GetById(siteLinkId);

            return Request.CreateResponse(response);
        }

        [Route("{type:int}/{ownerId:int}/{ownerType:int}"), HttpDelete]
        public HttpResponseMessage DeleteSiteLink(int type, int ownerId, int ownerType)
        {
            _siteLinksService.Delete(type, ownerId, ownerType);

            SuccessResponse response = new SuccessResponse();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}

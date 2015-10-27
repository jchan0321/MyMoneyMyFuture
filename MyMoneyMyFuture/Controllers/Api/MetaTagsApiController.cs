using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models.Requests;
using MyMoneyMyFuture.Models.Response;
using MyMoneyMyFuture.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace MyMoneyMyFuture.Controllers.Api
{
    public class MetaTagsApiController
    {
        private IUserService _userService;
        private IMetaTagsService _metaTagsService;
        private object ModelState;

        public MetaTagsApiController(IUserService service, IMetaTagsService mservice)
        {
            this._userService = service;
            this._metaTagsService = mservice;
        }

        [Route, HttpPost]
        public HttpResponseMessage AddPageMetaTag(PageMetaTagsAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string userId = _userService.GetCurrentUserId();
            ItemResponse<Int32> response = new ItemResponse<Int32>();

            response.Item = _metaTagsService.Add(model, userId);

            return Request.CreateResponse(response);
        }

        [Route("{pageMetaTagId:int}"), HttpPut]
        public HttpResponseMessage UpdatePageMetaTag(Models.Requests.PageMetaTagsUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse response = new SuccessResponse();

            _metaTagsService.Update(model);

            return Request.CreateResponse(response);
        }

        [Route(), HttpGet]
        public HttpResponseMessage GetMetaTag()
        {
            ItemsResponse<MetaTag> response = new ItemsResponse<MetaTag>();

            response.Items = _metaTagsService.GetMT();

            return Request.CreateResponse(response);
        }

        [Route("{ownerId:int}/{ownerType:int}/existing"), HttpGet]
        public HttpResponseMessage GetPageMetaTag(int ownerId, int ownerType)
        {
            ItemsResponse<PageMetaTag> response = new ItemsResponse<PageMetaTag>();

            response.Items = _metaTagsService.GetPageMT(ownerId, ownerType);

            return Request.CreateResponse(response);
        }

        [Route("{ownerId:int}/{ownerType:int}"), HttpGet]
        public HttpResponseMessage GetPageMetaTagValue(int ownerId, int ownerType)
        {
            ItemsResponse<PageMetaTagValue> response = new ItemsResponse<PageMetaTagValue>();

            response.Items = _metaTagsService.GetPageMTValue(ownerId, ownerType);

            return Request.CreateResponse(response);
        }

        [Route("{pageMetaTagId:int}"), HttpGet]
        public HttpResponseMessage GetPageMetaTagById(int pageMetaTagId)
        {
            ItemResponse<PageMetaTag> response = new ItemResponse<PageMetaTag>();

            response.Item = _metaTagsService.GetById(pageMetaTagId);

            return Request.CreateResponse(response);
        }

        [Route("{pageMetaTagId:int}"), HttpDelete]
        public HttpResponseMessage DeletePageMetaTag(int pageMetaTagId)
        {
            _metaTagsService.Delete(pageMetaTagId);

            SuccessResponse response = new SuccessResponse();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
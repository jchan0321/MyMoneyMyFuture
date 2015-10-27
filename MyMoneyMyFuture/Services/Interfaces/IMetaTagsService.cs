using System.Collections.Generic;
using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models.Requests;

namespace MyMoneyMyFuture.Services
{
    public interface IMetaTagsService
    {
        int Add(PageMetaTagsAddRequest model, string userId);
        void Delete(int id);
        PageMetaTag GetById(int Id);
        List<MetaTag> GetMT();
        List<PageMetaTag> GetPageMT(int OwnerId, int OwnerType);
        List<PageMetaTagValue> GetPageMTValue(int OwnerId, int OwnerType);
        void Update(PageMetaTagsUpdateRequest model);
    }
}
using System.Collections.Generic;
using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models.Requests;

namespace MyMoneyMyFuture.Services
{
    public interface ISiteLinksService
    {
        int Add(SiteLinksAddRequest model, string userId);
        void Delete(int type, int ownerId, int ownerType);
        SiteLink GetById(int Id);
        List<SiteLink> GetByOwner(int OwnerId, int OwnerType);
        void Update(SiteLinksUpdateRequest model);
    }
}
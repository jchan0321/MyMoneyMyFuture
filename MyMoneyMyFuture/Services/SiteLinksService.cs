using MyMoneyMyFuture.Data;
using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Services
{
    public class SiteLinksService : BaseService, ISiteLinksService
    {
        public Int32 Add(SiteLinksAddRequest model, string userId)
        {
            Int32 id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.SiteLinks_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Url", model.Url);
                   paramCollection.AddWithValue("@Type", model.Type);
                   paramCollection.AddWithValue("@Group", model.Group);
                   paramCollection.AddWithValue("@OwnerId", model.OwnerId);
                   paramCollection.AddWithValue("@OwnerType", model.OwnerType);
                   paramCollection.AddWithValue("@Userid", userId);


                   SqlParameter s = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   s.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(s);
               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   Int32.TryParse(param["@Id"].Value.ToString(), out id);
               }
               );

            return id;
        }

        public void Update(SiteLinksUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.SiteLinks_Update"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Url", model.Url);
                   paramCollection.AddWithValue("@Type", model.Type);
                   paramCollection.AddWithValue("@Group", model.Group);
                   paramCollection.AddWithValue("@OwnerId", model.OwnerId);
                   paramCollection.AddWithValue("@OwnerType", model.OwnerType);
                   paramCollection.AddWithValue("@Id", model.Id);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
               }
               );
        }

        public List<SiteLink> GetByOwner(int OwnerId, int OwnerType)
        {
            List<SiteLink> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.SiteLinks_Select"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@OwnerId", OwnerId);
                   paramCollection.AddWithValue("@OwnerType", OwnerType);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   SiteLink s = MapSiteLink(reader);

                   if (list == null)
                   {
                       list = new List<SiteLink>();
                   }

                   list.Add(s);
               }
               );

            return list;
        }

        public SiteLink GetById(int Id)
        {
            SiteLink s = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.SiteLinks_SelectById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   s = MapSiteLink(reader);

               }
               );

            return s;
        }

        private SiteLink MapSiteLink(IDataReader reader)
        {
            SiteLink s = new SiteLink();
            int startingIndex = 0;


            s.Id = reader.GetSafeInt32(startingIndex++);
            s.Url = reader.GetSafeString(startingIndex++);
            s.Type = reader.GetSafeInt32(startingIndex++);
            s.Group = reader.GetSafeInt16(startingIndex++);
            s.OwnerId = reader.GetSafeInt32(startingIndex++);
            s.OwnerType = reader.GetSafeInt32(startingIndex++);
            s.DateAdded = reader.GetSafeDateTime(startingIndex++);
            s.DateModified = reader.GetSafeDateTime(startingIndex++);
            s.LanguageCode = reader.GetSafeString(startingIndex++);
            return s;
        }

        public void Delete(int type, int ownerId, int ownerType)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.SiteLinks_Delete"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Type", type);
                   paramCollection.AddWithValue("@OwnerId", ownerId);
                   paramCollection.AddWithValue("@OwnerType", ownerType);

               });
        }
    }
}
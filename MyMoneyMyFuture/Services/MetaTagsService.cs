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
    public class MetaTagsService : BaseService, IMetaTagsService
    {
        public Int32 Add(PageMetaTagsAddRequest model, string userId)
        {
            Int32 id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.PageMetaTags_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@MetaTagId", model.MetaTagId);
                   paramCollection.AddWithValue("@Value", model.Value);
                   paramCollection.AddWithValue("@OwnerId", model.OwnerId);
                   paramCollection.AddWithValue("@OwnerType", model.OwnerType);
                   paramCollection.AddWithValue("@Userid", userId);


                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);
               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   Int32.TryParse(param["@Id"].Value.ToString(), out id);
               }
               );

            return id;
        }

        public void Update(PageMetaTagsUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.PageMetaTags_Update"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@MetaTagId", model.MetaTagId);
                   paramCollection.AddWithValue("@Value", model.Value);
                   paramCollection.AddWithValue("@OwnerId", model.OwnerId);
                   paramCollection.AddWithValue("@OwnerType", model.OwnerType);
                   paramCollection.AddWithValue("@Id", model.Id);


               }, returnParameters: delegate (SqlParameterCollection param)
               {
               }
               );
        }

        public List<MetaTag> GetMT()
        {
            List<MetaTag> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.MetaTags_Select"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

               }
               , map: delegate (IDataReader reader, short set)
               {
                   MetaTag m = new MetaTag();
                   MapMetaTag(reader, m);

                   if (list == null)
                   {
                       list = new List<MetaTag>();
                   }

                   list.Add(m);
               }
               );


            return list;
        }


        public List<PageMetaTag> GetPageMT(int OwnerId, int OwnerType)
        {
            List<PageMetaTag> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.PageMetaTags_Select"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@OwnerId", OwnerId);
                   paramCollection.AddWithValue("@OwnerType", OwnerType);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   PageMetaTag p = MapPageMetaTag(reader);

                   if (list == null)
                   {
                       list = new List<PageMetaTag>();
                   }

                   list.Add(p);
               }
               );

            return list;
        }

        public List<PageMetaTagValue> GetPageMTValue(int OwnerId, int OwnerType)
        {
            List<PageMetaTagValue> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.PageMetaTags_SelectByOwner"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@OwnerId", OwnerId);
                   paramCollection.AddWithValue("@OwnerType", OwnerType);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   PageMetaTagValue p = MapPageMetaTagType(reader);

                   if (list == null)
                   {
                       list = new List<PageMetaTagValue>();
                   }

                   list.Add(p);
               }
               );

            return list;
        }

        public PageMetaTag GetById(int Id)
        {
            PageMetaTag p = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.PageMetaTags_SelectById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   p = MapPageMetaTag(reader);
               }
               );

            return p;
        }

        private void MapMetaTag(IDataReader reader, MetaTag m)
        {
            int startingIndex = 0;

            m.Id = reader.GetSafeInt32(startingIndex++);
            m.Name = reader.GetSafeString(startingIndex++);
            m.Description = reader.GetSafeString(startingIndex++);
            m.Example = reader.GetSafeString(startingIndex++);
            m.Template = reader.GetSafeString(startingIndex++);
            m.DateAdded = reader.GetSafeDateTime(startingIndex++);
            m.DateModified = reader.GetSafeDateTime(startingIndex++);
            m.LanguageCode = reader.GetSafeString(startingIndex++);
        }

        private PageMetaTag MapPageMetaTag(IDataReader reader)
        {
            PageMetaTag p = new PageMetaTag();
            int startingIndex = 0; //startingOrdinal


            p.Id = reader.GetSafeInt32(startingIndex++);
            p.MetaTagId = reader.GetSafeInt32(startingIndex++);
            p.Value = reader.GetSafeString(startingIndex++);
            p.OwnerId = reader.GetSafeInt32(startingIndex++);
            p.OwnerType = reader.GetSafeInt32(startingIndex++);
            p.DateAdded = reader.GetSafeDateTime(startingIndex++);
            p.DateModified = reader.GetSafeDateTime(startingIndex++);
            p.LanguageCode = reader.GetSafeString(startingIndex++);
            return p;
        }

        private PageMetaTagValue MapPageMetaTagType(IDataReader reader)
        {
            PageMetaTagValue p = new PageMetaTagValue();
            int startingIndex = 0; //startingOrdinal

            p.PageId = reader.GetSafeInt32(startingIndex++);
            p.Value = reader.GetSafeString(startingIndex++);
            p.Id = reader.GetSafeInt32(startingIndex++);
            p.Name = reader.GetSafeString(startingIndex++);
            p.Template = reader.GetSafeString(startingIndex++);
            p.Description = reader.GetSafeString(startingIndex++);
            p.Example = reader.GetSafeString(startingIndex++);
            return p;
        }

        public void Delete(int id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.PageMetaTags_Delete"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", id);

               });
        }
    }
}
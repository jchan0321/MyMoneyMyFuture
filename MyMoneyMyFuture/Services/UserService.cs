using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MyMoneyMyFuture.Data;
using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models;
using MyMoneyMyFuture.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace MyMoneyMyFuture.Services
{
    public class UserService : BaseService, IUserService
    {

        public int Add(UserAddRequest model, string userId)
        {
            int uid = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Users_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@FirstName", model.FirstName);
                   paramCollection.AddWithValue("@LastName", model.LastName);
                   paramCollection.AddWithValue("@Email", model.Email);
                   paramCollection.AddWithValue("@Phone", model.Phone);
                   paramCollection.AddWithValue("@Income", model.Income);
                   paramCollection.AddWithValue("@Gender", model.Gender);
                   paramCollection.AddWithValue("@Age", model.Age);
                   //paramCollection.AddWithValue("@NumberOfChildren", model.NumberOfChildren);
                   paramCollection.AddWithValue("@MaritalStatus", model.MaritalStatus);

                   paramCollection.AddWithValue("@UserId", userId);

                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }

               , returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out uid);
               }
               );


            return uid;
        }



        public int CountConfirmedPhones()
        {
            int c = 0;

            DataProvider.ExecuteCmd(GetConnection, "dbo.AspNetUsers_CountConfirmedPhones"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
               }
               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0;

                   c = reader.GetSafeInt32(startingIndex++);

               });

            return c;
        }

        public List<User> GetConfirmedPhones()
        {
            List<User> list = new List<User>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.AspNetUsers_SelectConfirmedPhones"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
               }
               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0;
                   User u = new User();

                   u.Handle = reader.GetSafeString(startingIndex++);
                   u.Phone = reader.GetSafeString(startingIndex++);
                   u.UserId = reader.GetSafeString(startingIndex++);

                   if (list == null)
                   {
                       list = new List<User>();
                   }

                   list.Add(u);

               });

            return list;
        }

        public void SendTextMessage(TextMessage model)
        {
            List<User> list = new List<User>();

            list = GetConfirmedPhones();

            var twilio = new TwilioRestClient(_AccountSid, _AuthToken);

            foreach (var item in list)
            {
                string phoneWithCode = "+1" + item.Phone;

                var message = twilio.SendMessage(_TwilioNumber, phoneWithCode, model.Message);

                TextMessageLogInsert(item.UserId, item.Phone, null, model.Message);
            }

        }

        public void UpdatePhone(PhoneAddRequest model, string userId)
        {
            if (model.Phone != null)
            {
                model.Phone = ParsePhone(model.Phone);
            }

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_UpdatePhoneNumber"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@PhoneNumber", model.Phone);
                   paramCollection.AddWithValue("@Id", userId);

               }

               , returnParameters: delegate (SqlParameterCollection param)
               {

               }
               );

        }

        public int GeneratePhoneVerificationCodeText(PhoneAddRequest model, string userId, string currentPhone)
        {
            Random rnd = new Random();
            int code = rnd.Next(1000, 10000);
            InsertCode(code, userId);

            //string toNumber = "+18186203478"; //delete when ready for production
            //string toNumber = "+19172946087"; //ramona's cell

            string toNumber = "+1" + model.Phone; //uncomment when ready for production

            var twilio = new TwilioRestClient(_AccountSid, _AuthToken);
            string messageBody = "Here is your verification code: " + code;

            var message = twilio.SendMessage(_TwilioNumber, toNumber
                , messageBody);

            string oldPhone = currentPhone;
            int logId = TextMessageLogInsert(userId, oldPhone
                , model.Phone, messageBody);

            return logId;
        }



        private void InsertCode(int code, string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_InsertPhoneVerificationCode"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@PhoneVerificationCode", code);
                   paramCollection.AddWithValue("@Id", userId);

               }

               , returnParameters: delegate (SqlParameterCollection param)
               {

               }
               );
        }

        public string GetCurrentPhone(string userId)
        {
            string phone = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.AspNetUsers_SelectPhoneByUserId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)

               { paramCollection.AddWithValue("@Id", userId); }

               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0;

                   phone = reader.GetSafeString(startingIndex++);

               });

            return phone;
        }

        private int TextMessageLogInsert(string userId, string oldPhone, string newPhone, string message)
        {

            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.TextMessageLog_Insert"
                   , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                   {
                       paramCollection.AddWithValue("@UserId", userId);
                       paramCollection.AddWithValue("@OldPhone", oldPhone);
                       paramCollection.AddWithValue("@NewPhone", newPhone);
                       paramCollection.AddWithValue("@TextMessage", message);

                       SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                       p.Direction = System.Data.ParameterDirection.Output;

                       paramCollection.Add(p);
                   }

                   , returnParameters: delegate (SqlParameterCollection param)
                   {
                       int.TryParse(param["@Id"].Value.ToString(), out id);
                   }
                   );

            return id;
        }

        public bool VerifyCode(VerifyPhoneAddRequest model, string userId)
        {
            bool verified = false;


            DataProvider.ExecuteNonQuery(GetConnection, "dbo.AspNetUsers_VerifyPhoneCode"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", userId);
                   paramCollection.AddWithValue("@Code", model.VerificationCode);

                   SqlParameter p = new SqlParameter("@Verified", System.Data.SqlDbType.Bit);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);
               }

               , returnParameters: delegate (SqlParameterCollection param)
               {
                   bool.TryParse(param["@Verified"].Value.ToString(), out verified);
               }
               );

            return verified;
        }


        public string GetUsernames()
        {
            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";
            string response = cli.UploadString("http://www.spinxo.com/services/NameService.asmx/GetNames", "{snr:{Stub:\"username\"}}");

            return response;
        }



        public void Update(UserUpdateRequest model, string userId)
        {
            int uid = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Users_Update_v2"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@FirstName", model.FirstName);
                   paramCollection.AddWithValue("@LastName", model.LastName);
                   paramCollection.AddWithValue("@Email", model.Email);
                   paramCollection.AddWithValue("@Phone", model.Phone);
                   //paramCollection.AddWithValue("@Zip", model.Zip);
                   paramCollection.AddWithValue("@Gender", model.Gender);
                   paramCollection.AddWithValue("@Age", model.Age);
                   //paramCollection.AddWithValue("@HasKids", model.HasKids);
                   //paramCollection.AddWithValue("@CollegeStudent", model.CollegeStudent);
                   paramCollection.AddWithValue("@MaritalStatus", model.MaritalStatus);
                   //paramCollection.AddWithValue("@SharesFinances", model.SharesFinances);
                   //paramCollection.AddWithValue("@EmploymentStatus", model.EmploymentStatus);
                   paramCollection.AddWithValue("@Income", model.Income);
                   //paramCollection.AddWithValue("@Expenses", model.Expenses);
                   //paramCollection.AddWithValue("@CreditCardDebt", model.CreditCardDebt);
                   //paramCollection.AddWithValue("@StudentLoanDebt", model.StudentLoanDebt);
                   //paramCollection.AddWithValue("@Savings", model.Savings);
                   //paramCollection.AddWithValue("@HomeStatus", model.HomeStatus);
                   //paramCollection.AddWithValue("@FinancialConcern", model.FinancialConcern);
                   paramCollection.AddWithValue("@UserId", userId);
                   paramCollection.AddWithValue("@Id", model.Id);

               }
               , returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out uid);
               }
               );
        }

        public User GetById(int Id)
        {
            User p = new User();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_SelectById_v2"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)

               { paramCollection.AddWithValue("@Id", Id); }

               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0; //startingOrdinal

                   p.Id = reader.GetSafeInt32(startingIndex++);
                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.Email = reader.GetSafeString(startingIndex++);
                   p.Phone = reader.GetSafeString(startingIndex++);
                   p.Income = reader.GetSafeDecimal(startingIndex++);
                   p.Gender = reader.GetSafeInt32(startingIndex++);
                   p.Age = reader.GetSafeInt32(startingIndex++);
                   p.NumberOfChildren = reader.GetSafeInt32(startingIndex++);
                   p.MaritalStatus = reader.GetSafeInt32(startingIndex++);
                   p.UserId = reader.GetSafeString(startingIndex++);
                   p.Expenses = reader.GetSafeDecimal(startingIndex++);
                   p.Savings = reader.GetSafeDecimal(startingIndex++);

               }
               );


            return p;
        }

        public User GetByUserId(string userId)
        {
            User p = new User();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_SelectByUserId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)

               { paramCollection.AddWithValue("@UserId", userId); }

               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0; //startingOrdinal

                   //startingIndex++;
                   p.Id = reader.GetSafeInt32(startingIndex++);
                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.Handle = reader.GetSafeString(startingIndex++);
                   p.Avatar = reader.GetSafeString(startingIndex++);
                   p.AboutMe = reader.GetSafeString(startingIndex++);
                   p.Path = reader.GetSafeString(startingIndex++);

                   p.Line1 = reader.GetSafeString(startingIndex++);
                   p.Line2 = reader.GetSafeString(startingIndex++);
                   p.City = reader.GetSafeString(startingIndex++);
                   p.StateProvinceID = reader.GetSafeInt32(startingIndex++);
                   p.StateProvinceCode = reader.GetSafeString(startingIndex++);
                   p.CountryRegionCode = reader.GetSafeString(startingIndex++);
                   p.Name = reader.GetSafeString(startingIndex++);
                   p.TerritoryID = reader.GetSafeInt32(startingIndex++);
                   p.Zip = reader.GetSafeString(startingIndex++);
                   p.Country = reader.GetSafeInt32(startingIndex++);
                   p.Phone = reader.GetSafeString(startingIndex++);
                   p.Email = reader.GetSafeString(startingIndex++);
                   p.Age = reader.GetSafeInt32(startingIndex++);
                   p.FinancialConcern = reader.GetSafeInt32(startingIndex++);
                   p.Gender = reader.GetSafeInt32(startingIndex++);
                   p.MaritalStatus = reader.GetSafeInt32(startingIndex++);
                   p.SharesFinances = reader.GetSafeInt32(startingIndex++);
                   p.HasKids = reader.GetSafeInt32(startingIndex++);
                   p.CollegeStudent = reader.GetSafeInt32(startingIndex++);
                   p.EmploymentStatus = reader.GetSafeInt32(startingIndex++);
                   p.HomeStatus = reader.GetSafeInt32(startingIndex++);
                   p.CreditCardDebt = reader.GetSafeInt32(startingIndex++);
                   p.StudentLoanDebt = reader.GetSafeInt32(startingIndex++);
                   p.Income = reader.GetSafeDecimal(startingIndex++);
                   p.Expenses = reader.GetSafeDecimal(startingIndex++);
                   p.Savings = reader.GetSafeDecimal(startingIndex++);
                   p.LevelOfCollege = reader.GetSafeInt32(startingIndex++);
                   // p.OnboardCompleted = reader.GetSafeBool(startingIndex++);

               }
               );

            return p;
        }

        public UserProfile GetByHandle(string userHandle)
        {
            UserProfile p = new UserProfile();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_SelectByHandle_v3"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)

               { paramCollection.AddWithValue("@Handle", userHandle); }

               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0; //startingOrdinal

                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.Handle = reader.GetSafeString(startingIndex++);
                   p.Avatar = reader.GetSafeString(startingIndex++);
                   p.AboutMe = reader.GetSafeString(startingIndex++);
                   p.Email = reader.GetSafeString(startingIndex++);
                   p.ProfilePic = reader.GetSafeString(startingIndex++);
                   p.Phone = reader.GetSafeString(startingIndex++);
                   p.isPhoneVerified = reader.GetSafeBool(startingIndex++);
                   p.Address = _addressMapper.MapAddressDomain(reader, startingIndex);

               });


            return p;
        }

        public List<User> GetAll()
        {
            List<User> list = new List<User>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_Select"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection) { }

               , map: delegate (IDataReader reader, short set)
               {
                   User p = new User();
                   int startingIndex = 0;

                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.Handle = reader.GetSafeString(startingIndex++);
                   p.Phone = reader.GetSafeString(startingIndex++);
                   p.Income = reader.GetSafeDecimal(startingIndex++);
                   p.Gender = reader.GetSafeInt32(startingIndex++);
                   p.Age = reader.GetSafeInt32(startingIndex++);
                   p.NumberOfChildren = reader.GetSafeInt32(startingIndex++);
                   p.MaritalStatus = reader.GetSafeInt32(startingIndex++);
                   p.UserId = reader.GetSafeString(startingIndex++);
                   p.EmailConfirmed = reader.GetSafeBool(startingIndex++);
                   p.Status = reader.GetSafeInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<User>();
                   }
                   list.Add(p);

               }
               );


            return list;
        }

        public List<User> GetAllByStatus(int status)
        {
            List<User> list = new List<User>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_SelectByStatus"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Status", status);
               }

               , map: delegate (IDataReader reader, short set)
               {
                   User p = new User();
                   int startingIndex = 0;

                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.Phone = reader.GetSafeString(startingIndex++);
                   p.Income = reader.GetSafeDecimal(startingIndex++);
                   p.Gender = reader.GetSafeInt32(startingIndex++);
                   p.Age = reader.GetSafeInt32(startingIndex++);
                   p.NumberOfChildren = reader.GetSafeInt32(startingIndex++);
                   p.MaritalStatus = reader.GetSafeInt32(startingIndex++);
                   p.UserId = reader.GetSafeString(startingIndex++);
                   p.EmailConfirmed = reader.GetSafeBool(startingIndex++);
                   p.Status = reader.GetSafeInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<User>();
                   }
                   list.Add(p);

               }
               );


            return list;
        }

        public List<User> GetAllByEmailConfirm(bool emailConfirmed)
        {
            List<User> list = new List<User>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_SelectByEmailConfirmed"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@EmailConfirmed", emailConfirmed);
               }

               , map: delegate (IDataReader reader, short set)
               {
                   User p = new User();
                   int startingIndex = 0;

                   p.FirstName = reader.GetSafeString(startingIndex++);
                   p.LastName = reader.GetSafeString(startingIndex++);
                   p.Handle = reader.GetSafeString(startingIndex++);
                   p.Phone = reader.GetSafeString(startingIndex++);
                   p.Income = reader.GetSafeDecimal(startingIndex++);
                   p.Gender = reader.GetSafeInt32(startingIndex++);
                   p.Age = reader.GetSafeInt32(startingIndex++);
                   p.NumberOfChildren = reader.GetSafeInt32(startingIndex++);
                   p.MaritalStatus = reader.GetSafeInt32(startingIndex++);
                   p.UserId = reader.GetSafeString(startingIndex++);
                   p.EmailConfirmed = reader.GetSafeBool(startingIndex++);
                   p.Status = reader.GetSafeInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<User>();
                   }
                   list.Add(p);

               }
               );


            return list;
        }

        public int DeleteById(int Id)
        {
            int uid = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Users_DeleteById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);

               }
               , returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out uid);
               }
               );

            return uid;
        }

        public bool ConfirmEmail(string Id)
        {
            bool result = false;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Users_ConfirmEmail"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);

               }
               , returnParameters: delegate (SqlParameterCollection param)
               {
                   result = true;
               }
               );

            return result;
        }

        public bool CheckEmail(string email)
        {
            bool result = false;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_CheckEmail"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Email", email);

               }
               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0; //startingOrdinal

                   result = reader.GetSafeBool(startingIndex++);


               }
               );

            return result;
        }

        public bool IsCurrentUser(string userHandle)
        {
            bool result = false;

            string userId = GetCurrentUserId();

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Users_IsCurrentUser"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Handle", userHandle);
                   paramCollection.AddWithValue("@UserId", userId);

                   SqlParameter p = new SqlParameter("@Result", System.Data.SqlDbType.Bit);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }

               , returnParameters: delegate (SqlParameterCollection param)
               {
                   bool.TryParse(param["@Result"].Value.ToString(), out result);
               }
               );

            return result;

        }
        public int CheckStatus(string email)
        {
            int result = 0;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_CheckStatus"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Email", email);

               }
               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0; //startingOrdinal

                   result = reader.GetSafeInt32(startingIndex++);


               }
               );

            return result;
        }
        public void BlockUser(string handle)
        {

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_BlockByHandle"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Handle", handle);

               }
               , map: delegate (IDataReader reader, short set)
               {





               }
               );


        }

        public void RemoveFlag(string handle)
        {

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_RemoveFlag"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Handle", handle);

               }
               , map: delegate (IDataReader reader, short set)
               {





               }
               );


        }

        public bool UserOnlineStatus(string Id, bool Status)
        {

            LiveChat liveChat = new LiveChat();

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.IsUserOnline_UpdateOnlineStatus"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);
                   paramCollection.AddWithValue("@OnlineStatus", Status);

               }

               , returnParameters: delegate (SqlParameterCollection param)
               {

               }
               );

            if (CheckUserRole() && !Status)
            {
                liveChat.AdminOnlineStatus(CheckAdminIsOnline());
            }
            else if (CheckUserRole() && Status)
            {
                liveChat.AdminOnlineStatus(Status);
            }

            return Status;

        }
        private bool CheckUserRole()
        {
            bool userIsAdmin = false;
            IdentityUser currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                List<IdentityUserRole> roles = currentUser.Roles.ToList();
                foreach (var role in roles)
                {
                    if (role.RoleId == _superAdmin || role.RoleId == _admin)
                    {
                        userIsAdmin = true;
                        break;
                    }
                }

            }

            return userIsAdmin;
        }

        public bool CheckAdminIsOnline()
        {
            bool AdminIsOnline = false;

            DataProvider.ExecuteCmd(GetConnection, "dbo.IsUserOnline_GetOnlineUsers"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

               }
               , map: delegate (IDataReader reader, short set)
               {
                   AdminIsOnline = true;
               }
               );



            return AdminIsOnline;
        }

        public void InactiveCheck()
        {
            DataProvider.ExecuteCmd(GetConnection, "dbo.IsUserOnline_InactiveCheck"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)

               { }

               , map: delegate (IDataReader reader, short set)
               {

               });
        }


        public void UpdateAdminsOnlineTimeStamp()
        {

            string userId = GetCurrentUserId();

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.IsUserOnline_UpdateTimestamp"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {

                   paramCollection.AddWithValue("@UserId", userId);

               }

               , returnParameters: delegate (SqlParameterCollection param)
               {

               }
               );

        }

        public void SetOnboardComplete(string userId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Users_SetOnboardTrue"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserId", userId);

               }
               , returnParameters: delegate (SqlParameterCollection param)
               {
               }
               );
        }
        public string GetUserIdByHandle(string handle)
        {
            string id = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Users_GetUserIdByHandle"
           , inputParamMapper: delegate (SqlParameterCollection paramCollection)
           {
               paramCollection.AddWithValue("@Handle", handle);

           }
           , map: delegate (IDataReader reader, short set)
           {

               int startingIndex = 0; //startingOrdinal

                   id = reader.GetSafeString(startingIndex++);


           }
           );
            return id;
        }


        // *****EXISTING CODE BELOW ***** //
        private static ApplicationUserManager GetUserManager()
        {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        // Strips non-numeric characters from phone
        private static string ParsePhone(string input)
        {
            string phone = new string(input.Where(c => char.IsDigit(c)).ToArray());

            if (phone.StartsWith("1"))
            {
                phone = phone.Substring(1);
            }

            return phone;
        }

        public IdentityUser CreateUser(string email, string password, string phone)
        {
            if (phone != null)
            {
                phone = ParsePhone(phone);
            }

            ApplicationUserManager userManager = GetUserManager();

            ApplicationUser newUser = new ApplicationUser { UserName = email, Email = email, LockoutEnabled = false, PhoneNumber = phone };
            IdentityResult result = null;
            try
            {
                result = userManager.Create(newUser, password);

            }
            catch
            {
                new IdentityResultException(result);
            }

            if (result.Succeeded)
            {
                return newUser;
            }
            else
            {
                throw new IdentityResultException(result);
            }
        }


        public bool Signin(string emailaddress, string password)
        {
            bool result = false;

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.Find(emailaddress, password);
            if (user != null)
            {
                ClaimsIdentity signin = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, signin);
                result = true;

            }
            return result;
        }

        public bool IsUser(string emailaddress)
        {
            bool result = false;

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindByEmail(emailaddress);


            if (user != null)
            {

                result = true;

            }

            return result;
        }

        public ApplicationUser GetUser(string emailaddress)
        {
            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindByEmail(emailaddress);

            return user;
        }


        public ApplicationUser GetUserById(string userId)
        {

            ApplicationUserManager userManager = GetUserManager();
            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;

            ApplicationUser user = userManager.FindById(userId);

            return user;
        }

        public List<File> GetProfilePictures(string userId)
        {
            List<File> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Files_SelectProfilePictureByUser"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserId", userId);
               }
               , map: delegate (IDataReader reader, short set)
               {

                   int startingIndex = 0;
                   File p = new File();
                   p.Id = reader.GetSafeInt32(startingIndex++);
                   p.Path = reader.GetSafeString(startingIndex++);


                   int fileType = reader.GetSafeInt32(startingIndex++);

                   FileType newFiletype;
                   Enum.TryParse(fileType.ToString(), out newFiletype);
                   p.FileType = newFiletype;



                   if (list == null)
                   {
                       list = new List<File>();
                   }
                   list.Add(p);

               }
               );

            return list;
        }

        public void DeleteProfilePic(int Id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Files_DeleteById"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", Id);

               });
        }

        public bool ChangePassWord(string userId, string newPassword)
        {
            bool result = false;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
            {
                throw new Exception("You must provide a userId and a password");
            }

            ApplicationUser user = GetUserById(userId);
            IdentityResult newPass = null;

            if (user != null)
            {

                ApplicationUserManager userManager = GetUserManager();

                userManager.RemovePassword(userId);
                newPass = userManager.AddPassword(userId, newPassword);

                result = newPass.Succeeded;

                if (!result)
                {

                    throw new IdentityResultException(newPass);
                }

                return result;
            }

            return result;
        }


        public bool Logout()
        {
            bool result = false;

            IdentityUser user = GetCurrentUser();

            if (user != null)
            {
                IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                result = true;
            }

            return result;
        }


        public IdentityUser GetCurrentUser()
        {
            if (!IsLoggedIn())
                return null;
            ApplicationUserManager userManager = GetUserManager();

            IdentityUser currentUserId = userManager.FindById(GetCurrentUserId());
            return currentUserId;
        }

        public string GetCurrentUserId()
        {
            //REMOVE the comment on the next line when login in is enabled.
            return HttpContext.Current.User.Identity.GetUserId();
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(GetCurrentUserId());

        }
    }
}
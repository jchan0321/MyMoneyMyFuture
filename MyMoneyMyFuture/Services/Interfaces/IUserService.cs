using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using MyMoneyMyFuture.Domain;
using MyMoneyMyFuture.Models;
using MyMoneyMyFuture.Models.Requests;

namespace MyMoneyMyFuture.Services
{
    public interface IUserService
    {
        int Add(UserAddRequest model, string userId);
        void BlockUser(string handle);
        bool ChangePassWord(string userId, string newPassword);
        bool CheckAdminIsOnline();
        bool CheckEmail(string email);
        int CheckStatus(string email);
        bool ConfirmEmail(string Id);
        int CountConfirmedPhones();
        IdentityUser CreateUser(string email, string password, string phone);
        int DeleteById(int Id);
        void DeleteProfilePic(int Id);
        int GeneratePhoneVerificationCodeText(PhoneAddRequest model, string userId, string currentPhone);
        List<User> GetAll();
        List<User> GetAllByEmailConfirm(bool emailConfirmed);
        List<User> GetAllByStatus(int status);
        UserProfile GetByHandle(string userHandle);
        User GetById(int Id);
        User GetByUserId(string userId);
        List<User> GetConfirmedPhones();
        string GetCurrentPhone(string userId);
        IdentityUser GetCurrentUser();
        string GetCurrentUserId();
        List<File> GetProfilePictures(string userId);
        ApplicationUser GetUser(string emailaddress);
        ApplicationUser GetUserById(string userId);
        string GetUserIdByHandle(string handle);
        string GetUsernames();
        void InactiveCheck();
        bool IsCurrentUser(string userHandle);
        bool IsLoggedIn();
        bool IsUser(string emailaddress);
        bool Logout();
        void RemoveFlag(string handle);
        void SendTextMessage(TextMessage model);
        void SetOnboardComplete(string userId);
        bool Signin(string emailaddress, string password);
        void Update(UserUpdateRequest model, string userId);
        void UpdateAdminsOnlineTimeStamp();
        void UpdatePhone(PhoneAddRequest model, string userId);
        bool UserOnlineStatus(string Id, bool Status);
        bool VerifyCode(VerifyPhoneAddRequest model, string userId);
    }
}
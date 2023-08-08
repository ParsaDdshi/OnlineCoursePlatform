using Microsoft.AspNetCore.Http;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.DataLayer.Entities.User;
using OnlineCoursePlatform.DataLayer.Entities.Wallet;

namespace OnlineCoursePlatform.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region Account

        bool IsUserNameExist(string userName);
        bool IsEmailExist(string email);
        int InsertUser(User user);
        User LoginUser(string email, string hashPassword);
        bool ActiveAccount(string activeCode);
        User GetUserByEmail(string email);
        User GetUserByUserId(int userId);
        User GetUserByActiveCode(string activeCode);
        User GetUserByUserName(string userName);

        #endregion

        #region User Panel

        UserInformationViewModel GetUserInformation(string userName);
        UserInformationViewModel GetUserInformation(int userId);
        UserPanelSideBarViewModel GetUserPanelSideBarData(string userName);
        EditProfileViewModel GetDataForUserProfileEdit(string userName);
        void EditProfile(string userName, EditProfileViewModel editProfileViewModel);
        void DeActiveAccount(string userName);
        bool CompareOldPassword(string userName, string oldPassword);
        void ChangeUserPassword(string userName, string newPassword);
        int GetUserIdByUserName(string userName);

        #endregion

        #region Wallet

        int UserWalletBalance(string userName);
        List<WalletViewModel> GetUserWallet(string userName);
        int ChargeWallet(string userName, int amount,string description, bool isPay = false);
        int AddWallet(Wallet wallet);
        Wallet GetWalletByWalletId(int walletId);

        #endregion

        #region Admin Panel

        UsersForAdminViewModel GetUsers(string filterEmail = "", string filterUserName = "", int pageId = 1);
        UsersForAdminViewModel GetDeletedUsers(string filterEmail = "", string filterUserName = "", int pageId = 1);
        int InsertUserFromAdmin(CreateUserViewModel createUserViewModel);
        EditUserViewModel GetUserForShowInEditMode(int userId);
        void EditUserFromAdmin(EditUserViewModel editUserViewModel, bool ActiveDeActive);
        void SaveUserAvatar(IFormFile UserAvatar, User user);
        List<int> GetUserRoles(int userId);
        void DeleteUser(int userId);

        #endregion
        void Save();
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.Core.Generators;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Context;
using OnlineCoursePlatform.DataLayer.Entities.User;
using OnlineCoursePlatform.DataLayer.Entities.Wallet;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace OnlineCoursePlatform.Core.Services
{
    public class UserService : IUserService
    {
        private readonly OCPContext _context;
        public UserService(OCPContext context) => _context = context;

        public bool ActiveAccount(string activeCode)
        {
            User user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsActive)
                return false;

            user.IsActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            Save();
            return true;
        }

        public User GetUserByActiveCode(string activeCode) => _context.Users
            .SingleOrDefault(u => u.ActiveCode == activeCode);

        public User GetUserByEmail(string email) => _context.Users
            .SingleOrDefault(u => u.Email == email);

        public User GetUserByUserName(string userName) => _context.Users
            .SingleOrDefault(u => u.UserName == userName);



        public int InsertUser(User user)
        {
            _context.Users.Add(user);
            Save();
            return user.UserId;
        }

        public bool IsEmailExist(string email) => _context.Users.Any(u => u.Email == email);

        public bool IsUserNameExist(string userName) => _context.Users.Any(u => u.UserName == userName);

        public User LoginUser(string email, string hashPassword) => _context.Users
            .SingleOrDefault(u => u.Email == email && u.Password == hashPassword);

        public UserInformationViewModel GetUserInformation(string userName) => _context.Users
                .Where(u => u.UserName == userName).Select(u => new UserInformationViewModel
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    RegisterDate = u.RegisterDate,
                    Wallet = UserWalletBalance(userName)
                }).Single();

        public void Save() => _context.SaveChanges();

        public UserPanelSideBarViewModel GetUserPanelSideBarData(string userName) => _context.Users.
            Where(u => u.UserName == userName).Select(u => new UserPanelSideBarViewModel
            {
                UserName = u.UserName,
                ImageName = u.UserAvatar,
                RegisterDate = u.RegisterDate
            }).Single();

        public EditProfileViewModel GetDataForUserProfileEdit(string userName) => _context.Users.
            Where(u => u.UserName == userName).Select(u => new EditProfileViewModel
            {
                UserName = u.UserName,
                AvatarName = u.UserAvatar,
                Email = u.Email
            }).Single();

        public void EditProfile(string userName, EditProfileViewModel editProfileViewModel)
        {
            User user = GetUserByUserName(userName);

            if (editProfileViewModel.UserAvatar != null)
            {
                string imagePath = "";
                if (editProfileViewModel.AvatarName != "DefaultAvatar.png")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar",
                        user.UserAvatar);
                    if (File.Exists(imagePath))
                        File.Delete(imagePath);
                }
                editProfileViewModel.AvatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(editProfileViewModel.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editProfileViewModel.AvatarName);
                using (FileStream stream = new FileStream(imagePath, FileMode.Create))
                    editProfileViewModel.UserAvatar.CopyTo(stream);
            }

            user.UserName = editProfileViewModel.UserName;
            user.Email = FixedText.FixedEmail(editProfileViewModel.Email);
            user.UserAvatar = editProfileViewModel.AvatarName;
            Save();
        }

        public void DeActiveAccount(string userName)
        {
            User user = GetUserByUserName(userName);
            user.IsActive = false;
            Save();
        }

        public bool CompareOldPassword(string userName, string oldPassword) => _context.Users.Any
            (u => u.UserName == userName && u.Password == PasswordHelper.EncodePasswordMd5(oldPassword));

        public void ChangeUserPassword(string userName, string newPassword)
        {
            User user = GetUserByUserName(userName);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            Save();
        }

        public int UserWalletBalance(string userName)
        {
            int userId = GetUserIdByUserName(userName);

            var Deposit = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPay)
                .Select(w => w.Amount).ToList();

            var Withdraw = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 2)
                .Select(w => w.Amount).ToList();

            return (Deposit.Sum() - Withdraw.Sum());
        }

        public int GetUserIdByUserName(string userName) => _context.Users
            .Single(u => u.UserName == userName).UserId;

        public List<WalletViewModel> GetUserWallet(string userName) => _context.Wallets
            .Where(w => w.UserId == GetUserIdByUserName(userName) && w.IsPay)
            .Select(w => new WalletViewModel()
            {
                Amount = w.Amount,
                DateTime = w.CreateDate,
                Description = w.Description,
                Type = w.TypeId
            }).ToList();

        public int ChargeWallet(string userName, int amount, string description, bool isPay = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByUserName(userName)
            };
            return AddWallet(wallet);
        }

        public int AddWallet(Wallet wallet)
        {
            _context.Add(wallet);
            Save();
            return wallet.WalletId;
        }

        public Wallet GetWalletByWalletId(int walletId) => _context.Wallets.Find(walletId);

        public UsersForAdminViewModel GetUsers(string filterEmail = "", string filterUserName = "", int pageId = 1)
        {
            IQueryable<User> result = _context.Users;

            if (!string.IsNullOrEmpty(filterEmail))
                result = result.Where(u => u.Email.Contains(filterEmail));


            if (!string.IsNullOrEmpty(filterUserName))
                result = result.Where(u => u.UserName.Contains(filterUserName));

            // Show Item In Page
            int take = 20;
            int skip = (pageId - 1) * take;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public int InsertUserFromAdmin(CreateUserViewModel createUserViewModel)
        {
            User user = new User()
            {
                Password = PasswordHelper.EncodePasswordMd5(createUserViewModel.Password),
                Email = FixedText.FixedEmail(createUserViewModel.Email),
                IsActive = true,
                RegisterDate = DateTime.Now,
                ActiveCode = NameGenerator.GenerateUniqCode(),
                UserName = createUserViewModel.UserName,
            };


            #region Save Avatar
            if (createUserViewModel.UserAvatar != null)
            {
                SaveUserAvatar(createUserViewModel.UserAvatar, user);
            }
            else
                user.UserAvatar = "DefaultAvatar.png";
            #endregion

            return InsertUser(user);

        }

        public EditUserViewModel GetUserForShowInEditMode(int userId) => _context.Users.Where(u => u.UserId == userId)
            .Select(u => new EditUserViewModel()
            {
                UserId = u.UserId,
                UserName = u.UserName,
                PreviousUserName = u.UserName,
                Email = u.Email,
                PreviousEmail = u.Email,
                AvatarName = u.UserAvatar,
                IsActive = u.IsActive
            }).Single();

        public void EditUserFromAdmin(EditUserViewModel editUserViewModel, bool ActiveDeActive)
        {
            User user = GetUserByUserId(editUserViewModel.UserId);
            user.UserName = editUserViewModel.UserName;
            user.Email = editUserViewModel.Email;
            user.IsActive = ActiveDeActive;

            if (!string.IsNullOrEmpty(editUserViewModel.Password))
                user.Password = PasswordHelper.EncodePasswordMd5(editUserViewModel.Password);

            if (editUserViewModel.UserAvatar != null)
            {
                //Delete Old Profile
                if (editUserViewModel.AvatarName != "DefaultAvatar.png")
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editUserViewModel.AvatarName);
                    if (File.Exists(deletePath))
                        File.Delete(deletePath);
                }

                SaveUserAvatar(editUserViewModel.UserAvatar, user);
            }
            Save();
        }

        public User GetUserByUserId(int userId) => _context.Users.SingleOrDefault(u => u.UserId == userId);

        public void SaveUserAvatar(IFormFile UserAvatar, User user)
        {
            string imagePath = "";
            user.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(UserAvatar.FileName);
            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", user.UserAvatar);
            using (FileStream stream = new FileStream(imagePath, FileMode.Create))
                UserAvatar.CopyTo(stream);
        }

        public List<int> GetUserRoles(int userId) => _context.UsersRoles
            .Where(r => r.UserId == userId).ToList().Select(r => r.RoleId).ToList();

        public UsersForAdminViewModel GetDeletedUsers(string filterEmail = "", string filterUserName = "", int pageId = 1)
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);

            if (!string.IsNullOrEmpty(filterEmail))
                result = result.Where(u => u.Email.Contains(filterEmail));


            if (!string.IsNullOrEmpty(filterUserName))
                result = result.Where(u => u.UserName.Contains(filterUserName));

            // Show Item In Page
            int take = 20;
            int skip = (pageId - 1) * take;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserByUserId(userId);
            user.IsDelete = true;
            Save();
        }

        public UserInformationViewModel GetUserInformation(int userId)
        {
            User user = GetUserByUserId(userId);
            UserInformationViewModel userInformationViewModel = new UserInformationViewModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                RegisterDate = user.RegisterDate,
                Wallet = UserWalletBalance(user.UserName)
            };
            return userInformationViewModel;
        }

    }
}

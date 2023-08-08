using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.Pages.Admin.Users
{
    [PermissionChecker(6)]
    public class DeletedUserListModel : PageModel
    {
        private readonly IUserService _userService;
        public DeletedUserListModel(IUserService userService) => _userService = userService;

        public UsersForAdminViewModel UsersForAdminViewModel { get; set; }
        public void OnGet(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            UsersForAdminViewModel = _userService.GetDeletedUsers(filterEmail, filterUserName, pageId);
        }
    }
}

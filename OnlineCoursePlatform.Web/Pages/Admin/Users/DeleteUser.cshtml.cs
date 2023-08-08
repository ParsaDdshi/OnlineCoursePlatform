using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.Pages.Admin.Users
{
    [PermissionChecker(5)]
    public class DeleteUserModel : PageModel
    {
        private readonly IUserService _userService;
        public DeleteUserModel(IUserService userService) => _userService = userService;

        public UserInformationViewModel UserInformationViewModel { get; set; }
        public void OnGet(int id)
        {
            ViewData["UserId"] = id;
            UserInformationViewModel = _userService.GetUserInformation(id);
        }

        public IActionResult OnPost(int userId)
        {
            _userService.DeleteUser(userId);
            return RedirectToPage("Index");
        }
    }
}

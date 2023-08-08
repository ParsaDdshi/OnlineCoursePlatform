using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;

namespace OnlineCoursePlatform.Web.Pages.Admin.Users
{
    [PermissionChecker(3)]
    public class CreateUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }
        public void OnGet()
        {
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> selectedRoles)
        {
            ViewData["Roles"] = _permissionService.GetRoles();
            if (!ModelState.IsValid)
                return Page();

            if (_userService.IsUserNameExist(CreateUserViewModel.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاریری معتبر نمی باشد");
                return Page();
            }

            if (_userService.IsEmailExist(FixedText.FixedEmail(CreateUserViewModel.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return Page();
            }

            int userId = _userService.InsertUserFromAdmin(CreateUserViewModel);
            
            //Add User Roles
            _permissionService.AddRolesToUser(selectedRoles, userId);

            return Redirect("/Admin/Users");
        }
    }
}

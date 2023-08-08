using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Convertors;
using OnlineCoursePlatform.Core.DTOs.User;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Senders;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;

namespace OnlineCoursePlatform.Web.Pages.Admin.Users
{
    [PermissionChecker(4)]
    public class EditUserModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        public EditUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }
        [BindProperty]
        public List<int> SelectedRoles { get; set; }
        public List<Role> Roles { get; set; }

        public void OnGet(int id)
        {
            Roles = _permissionService.GetRoles();
            EditUserViewModel = _userService.GetUserForShowInEditMode(id);
            SelectedRoles = _userService.GetUserRoles(EditUserViewModel.UserId);
        }

        public IActionResult OnPost(bool ActiveDeActive)
        {
            Roles = _permissionService.GetRoles();
            if (!ModelState.IsValid)
            {
                SelectedRoles = _userService.GetUserRoles(EditUserViewModel.UserId);
                return Page();
            }


            if (EditUserViewModel.PreviousUserName != EditUserViewModel.UserName)
            {
                if (_userService.IsUserNameExist(EditUserViewModel.UserName))
                {
                    ModelState.AddModelError("UserName", "نام کاریری تکراری است ");
                    return Page();
                }
            }

            if (EditUserViewModel.PreviousEmail != EditUserViewModel.Email)
            {
                if (_userService.IsEmailExist(FixedText.FixedEmail(EditUserViewModel.Email)))
                {
                    ModelState.AddModelError("Email", "ایمیل تکراری است");
                    return Page();
                }
            }

            _userService.EditUserFromAdmin(EditUserViewModel, ActiveDeActive);
            _permissionService.EditUserRoles(SelectedRoles, EditUserViewModel.UserId);

            return RedirectToPage("Index");
        }
    }
}

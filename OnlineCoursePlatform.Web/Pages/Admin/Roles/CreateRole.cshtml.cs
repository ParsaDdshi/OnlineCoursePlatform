using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;
using System.Security;

namespace OnlineCoursePlatform.Web.Pages.Admin.Roles
{
    [PermissionChecker(8)]
    public class CreateRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public CreateRoleModel(IPermissionService permissionService) => _permissionService = permissionService;

        [BindProperty]
        public Role Role { get; set; }
        public void OnGet()
        {
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
        }

        public IActionResult OnPost(List<int> selectedPermissions)
        {
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
            if (!ModelState.IsValid)
                return Page();

            Role.IsDelete = false;
            int roleId = _permissionService.AddRole(Role);

            _permissionService.AddPermissionsToRole(roleId, selectedPermissions);

            return RedirectToPage("Index");
        }
    }
}

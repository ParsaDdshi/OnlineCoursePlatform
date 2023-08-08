using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;

namespace OnlineCoursePlatform.Web.Pages.Admin.Roles
{
    [PermissionChecker(9)]
    public class EditRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public EditRoleModel(IPermissionService permissionService) => _permissionService = permissionService;

        [BindProperty]
        public Role Role { get; set; }
        public void OnGet(int id)
        {
            Role = _permissionService.GetRoleById(id);
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
            ViewData["RolePermissions"] = _permissionService.GetRolePermissions(id);
        }

        public IActionResult OnPost(List<int> selectedPermissions, int id)
        {
            ViewData["Permissions"] = _permissionService.GetAllPermissions();
            ViewData["RolePermissions"] = _permissionService.GetRolePermissions(id);
            if (!ModelState.IsValid)
                return Page();

            _permissionService.UpdateRole(Role);
            _permissionService.UpdateRolePermissions(id, selectedPermissions);

            return RedirectToPage("Index");
        }
    }
}

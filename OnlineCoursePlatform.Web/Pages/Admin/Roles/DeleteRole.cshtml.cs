using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;

namespace OnlineCoursePlatform.Web.Pages.Admin.Roles
{
    [PermissionChecker(10)]
    public class DeleteRoleModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public DeleteRoleModel(IPermissionService permissionService) => _permissionService = permissionService;

        [BindProperty]
        public Role Role { get; set; }
        public void OnGet(int id)
        {
            Role = _permissionService.GetRoleById(id);
        }

        public IActionResult OnPost()
        {
            _permissionService.DeleteRole(Role);
            return RedirectToPage("Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;

namespace OnlineCoursePlatform.Web.Pages.Admin.Roles
{
    [PermissionChecker(7)]
    public class IndexModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public IndexModel(IPermissionService permissionService) => _permissionService = permissionService;

        public List<Role> RolesList { get; set; }
        public void OnGet()
        {
            RolesList = _permissionService.GetRoles();
        }
    }
}

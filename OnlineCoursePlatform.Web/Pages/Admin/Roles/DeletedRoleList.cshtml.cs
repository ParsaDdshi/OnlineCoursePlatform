using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCoursePlatform.Core.Security;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Entities.User;

namespace OnlineCoursePlatform.Web.Pages.Admin.Roles
{
    [PermissionChecker(11)]
    public class DeletedRoleListModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public DeletedRoleListModel(IPermissionService permissionService) => _permissionService = permissionService;

        public List<Role> RolesList { get; set; }
        public void OnGet()
        {
            RolesList = _permissionService.GetDeletedRoles();
        }
    }
}

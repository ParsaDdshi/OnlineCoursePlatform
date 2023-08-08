using OnlineCoursePlatform.DataLayer.Entities.Permission;
using OnlineCoursePlatform.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        List<Role> GetRoles();
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);
        int AddRole(Role role);
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditUserRoles(List<int> roleIds, int userId);
        void Save();
        List<Role> GetDeletedRoles();

        #endregion

        #region Permissions

        List<Permission> GetAllPermissions();
        void AddPermissionsToRole(int roleId, List<int> permissions);
        List<int> GetRolePermissions(int roleId);
        void UpdateRolePermissions(int roleId, List<int> permissions);
        bool CheckPermission(int permissionId, string userName);

        #endregion
    }
}

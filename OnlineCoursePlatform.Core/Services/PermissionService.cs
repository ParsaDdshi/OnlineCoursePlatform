using Microsoft.EntityFrameworkCore;
using OnlineCoursePlatform.Core.Services.Interfaces;
using OnlineCoursePlatform.DataLayer.Context;
using OnlineCoursePlatform.DataLayer.Entities.Permission;
using OnlineCoursePlatform.DataLayer.Entities.User;

namespace OnlineCoursePlatform.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly OCPContext _context;
        private readonly IUserService _userService;
        public PermissionService(OCPContext context, IUserService userService) 
        {
            _context = context;
            _userService = userService;
        } 

        public void AddPermissionsToRole(int roleId, List<int> permissions)
        {
            foreach(int permissionId in permissions)
            {
                _context.RolePermission.Add(new RolePermission()
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
            Save();
        }

        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            Save();
            return role.RoleId;
        }

        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach(int item in roleIds)
            {
                _context.UsersRoles.Add(new UserRole()
                {
                    RoleId = item,
                    UserId = userId
                });
            }
            Save();
        }

        public void DeleteRole(Role role)
        {
            role.IsDelete = true;
            UpdateRole(role);
        }

        public void EditUserRoles(List<int> roleIds, int userId)
        {
            _context.UsersRoles.Where(r => r.UserId == userId)
                .ToList().ForEach(r => _context.UsersRoles.Remove(r));
            AddRolesToUser(roleIds, userId);
        }

        public List<Permission> GetAllPermissions() => _context.Permission.ToList();

        public List<Role> GetDeletedRoles() => _context.Roles.IgnoreQueryFilters()
            .Where(r => r.IsDelete).ToList();

        public Role GetRoleById(int roleId) => _context.Roles.Find(roleId);

        public List<Role> GetRoles() => _context.Roles.ToList();

        public List<int> GetRolePermissions(int roleId) => _context.RolePermission
            .Where(p => p.RoleId == roleId).Select(p => p.PermissionId).ToList();

        public void Save() => _context.SaveChanges();

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            Save();
        }

        public void UpdateRolePermissions(int roleId, List<int> permissions)
        {
            _context.RolePermission.Where(p => p.RoleId == roleId).ToList()
                .ForEach(p => _context.RolePermission.Remove(p));

            AddPermissionsToRole(roleId, permissions);
        }

        public bool CheckPermission(int permissionId, string userName)
        {
            List<int> userRoles = _context.UsersRoles
                .Where(r => r.UserId == _userService.GetUserIdByUserName(userName))
                .Select(r => r.RoleId).ToList();

            if (!userRoles.Any())
                return false;

            List<int> rolePermissions = _context.RolePermission
                .Where(p => p.PermissionId == permissionId)
                .Select(p => p.RoleId).ToList();

            return rolePermissions.Any(p => userRoles.Contains(p));
        }
    }
}

using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Data.Repositories.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<List<RolePermission>> GetAllRolePermissionsAsync();
        Task CreateRolePermissionAsync(RolePermission permission);
        Task UpdateRolePermissionAsync(RolePermission permission);
    }
}

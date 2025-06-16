using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data.Repositories.Interfaces;
using MiniAccountManagementSystem.Models;
using System.Data;

namespace MiniAccountManagementSystem.Data.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly string _connectionString;

        public RolePermissionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<RolePermission>> GetAllRolePermissionsAsync()
        {
            var permissions = new List<RolePermission>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageRolePermissions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissions.Add(new RolePermission
                            {
                                RolePermissionId = reader.GetInt32("RolePermissionId"),
                                RoleId = reader.GetString("RoleId"),
                                RoleName = reader.GetString("RoleName"),
                                ModuleId = reader.GetInt32("ModuleId"),
                                ModuleName = reader.GetString("ModuleName"),
                                CanAccess = reader.GetBoolean("CanAccess")
                            });
                        }
                    }
                }
            }
            return permissions;
        }

        public async Task CreateRolePermissionAsync(RolePermission permission)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageRolePermissions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "INSERT");
                    command.Parameters.AddWithValue("@RoleId", permission.RoleId);
                    command.Parameters.AddWithValue("@ModuleId", permission.ModuleId);
                    command.Parameters.AddWithValue("@CanAccess", permission.CanAccess);

                    await command.ExecuteScalarAsync();
                }
            }
        }

        public async Task UpdateRolePermissionAsync(RolePermission permission)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_ManageRolePermissions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "UPDATE");
                    command.Parameters.AddWithValue("@RolePermissionId", permission.RolePermissionId);
                    command.Parameters.AddWithValue("@CanAccess", permission.CanAccess);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

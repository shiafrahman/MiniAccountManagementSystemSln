namespace MiniAccountManagementSystem.Models
{
    public class RolePermission
    {
        public int RolePermissionId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool CanAccess { get; set; }
    }
}

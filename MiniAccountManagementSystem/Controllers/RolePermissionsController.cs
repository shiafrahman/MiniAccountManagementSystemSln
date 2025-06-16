using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data.Repositories.Interfaces;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolePermissionsController : Controller
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _connectionString;

        public RolePermissionsController(IRolePermissionRepository rolePermissionRepository, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _roleManager = roleManager;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {
            var permissions = await _rolePermissionRepository.GetAllRolePermissionsAsync();
            return View(permissions);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Modules = await GetModulesAsync();
            return View(new RolePermission());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolePermission permission)
        {
            if (ModelState.IsValid)
            {
                await _rolePermissionRepository.CreateRolePermissionAsync(permission);
                TempData["Success"] = "Permission created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Modules = await GetModulesAsync();
            TempData["Error"] = "Failed to create permission. Please check the input.";
            return View(permission);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var permissions = await _rolePermissionRepository.GetAllRolePermissionsAsync();
            var permission = permissions.FirstOrDefault(p => p.RolePermissionId == id);
            if (permission == null)
            {
                return NotFound();
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Modules = await GetModulesAsync();
            return View(permission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RolePermission permission)
        {
            if (ModelState.IsValid)
            {
                await _rolePermissionRepository.UpdateRolePermissionAsync(permission);
                TempData["Success"] = "Permission updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Modules = await GetModulesAsync();
            TempData["Error"] = "Failed to update permission. Please check the input.";
            return View(permission);
        }

        private async Task<List<Module>> GetModulesAsync()
        {
            var modules = new List<Module>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT ModuleId, ModuleName FROM Modules", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            modules.Add(new Module
                            {
                                ModuleId = reader.GetInt32(reader.GetOrdinal("ModuleId")),
                                ModuleName = reader.GetString(reader.GetOrdinal("ModuleName"))
                            });
                        }
                    }
                }
            }
            return modules;
        }
    }

    
}

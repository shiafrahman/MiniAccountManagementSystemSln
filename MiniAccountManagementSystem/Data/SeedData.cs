using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;


namespace MiniAccountManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string[] roles = { "Admin", "Accountant", "Viewer" };

            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

           
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var modules = new[] { "ChartOfAccounts", "Vouchers", "RolePermissions" };
                foreach (var module in modules)
                {
                    using (var command = new SqlCommand("INSERT INTO Modules (ModuleName) SELECT @ModuleName WHERE NOT EXISTS (SELECT 1 FROM Modules WHERE ModuleName = @ModuleName)", connection))
                    {
                        command.Parameters.AddWithValue("@ModuleName", module);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}

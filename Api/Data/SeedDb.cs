using Api.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace Api.Data
{
  public class SeedDb
  {
    public static async Task CreateRolesSeed(
      RoleManager<IdentityRole> roleManager
    )
    {
      // Crear roles si no existen
      if (roleManager.Roles.Any()) return;

      await roleManager.CreateAsync(new IdentityRole(RoleConstants.ADMIN));
      await roleManager.CreateAsync(new IdentityRole(RoleConstants.MANAGER));
      await roleManager.CreateAsync(new IdentityRole(RoleConstants.BASIC));
    }

  }
}

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

      await roleManager.CreateAsync(new IdentityRole(Roles.ADMIN.ToString()));
      await roleManager.CreateAsync(new IdentityRole(Roles.BASIC.ToString()));
    }

  }
}

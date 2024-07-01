using Api.Common.Enums;
using Api.Models;
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

    public static async Task CreateUserAdminBasicSeed(
      UserManager<AppUser> userManager
    )
    {
      // Crear usuario administrador si no existe
      var userAdmin = await userManager.FindByEmailAsync("patriciomiguel_12@hotmail.com");

      if (userAdmin is not null) return;

      var user = new AppUser
      {
        UserName = "Pato",
        Email = "PatricioMiguel_12@hotmail.com",
        Names = "Patricio Miguel",
        PaternalLastName = "Osorio",
        MothersLastName = "Osorio",
        EmailConfirmed = true
      };

      await userManager.CreateAsync(user, "Pato123.");
      await userManager.AddToRoleAsync(user, RoleConstants.ADMIN);
      await userManager.AddToRoleAsync(user, RoleConstants.BASIC);
    }
  }
}

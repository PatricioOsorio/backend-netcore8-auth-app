using Api.Common.Enums;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    // api/account/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);


      var user = new AppUser
      {
        UserName = registerDto.Correo,
        Email = registerDto.Correo,
        Nombres = registerDto.Nombres,
        ApellidoPaterno = registerDto.ApellidoPaterno,
        ApellidoMaterno = registerDto.ApellidoMaterno
      };

      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) return BadRequest(result.Errors);

      if (registerDto.Roles == null)
      {
        await _userManager.AddToRoleAsync(user, Roles.BASIC.ToString());
      }
      else
      {
        foreach (var role in registerDto.Roles)
        {
          await _userManager.AddToRoleAsync(user, role.ToUpper());
        }
      }

      return Ok(new AuthRespondeDto
      {
        Isuccess = true,
        Message = "Usuario creado con éxito"
      });

    }
  }
}

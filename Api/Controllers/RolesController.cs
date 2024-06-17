using Api.Common.Enums;
using Api.Dtos;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
  [ApiController]
  [Authorize(Roles = "ADMIN, MANAGER")]
  [Route("api/[controller]")]
  public class RolesController : Controller
  {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
      _roleManager = roleManager;
      _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole(RoleDto roleDto)
    {
      if (string.IsNullOrEmpty(roleDto.RoleName)) return BadRequest(new ResponseDto { IsSuccess = false, Message = "El nombre del rol es requerido" });

      var roleExist = await _roleManager.RoleExistsAsync(roleDto.RoleName);

      if (roleExist) { return BadRequest(new ResponseDto { IsSuccess = false, Message = "El rol ya existe" }); }

      var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleDto.RoleName.ToUpper()));

      if (!roleResult.Succeeded) return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error al crear el rol" });

      return Ok(new ResponseDto { IsSuccess = true, Message = "Rol creado exitosamente" });
    }

    [HttpGet("getRoles")]
    public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
    {
      // lista de roles con la cantidad de usuarios asignados
      var roles = await _roleManager.Roles.Select(r => new RoleResponseDto
      {
        Id = r.Id,
        Name = r.Name,
        TotalUsers = _userManager.GetUsersInRoleAsync(r.Name!).Result.Count
      }).ToListAsync();

      return Ok(roles);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      var role = await _roleManager.FindByIdAsync(id);

      if (role == null) return NotFound(new ResponseDto { IsSuccess = false, Message = "Rol no encontrado" });

      var roleResult = await _roleManager.DeleteAsync(role);

      if (!roleResult.Succeeded) return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error al eliminar el rol" });

      return Ok(new ResponseDto { IsSuccess = true, Message = "Rol eliminado exitosamente" });
    }
  }
}

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
  [Authorize(Roles = $"{RoleConstants.ADMIN}")]
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
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
    {
      var roles = await _roleManager.Roles.ToListAsync();
      var rolesWithUserCounts = new List<RoleResponseDto>();

      foreach (var role in roles)
      {
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        rolesWithUserCounts.Add(new RoleResponseDto
        {
          Id = role.Id,
          Name = role.Name,
          TotalUsers = usersInRole.Count
        });
      }

      return Ok(rolesWithUserCounts);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      var role = await _roleManager.FindByIdAsync(id);

      if (role == null) return NotFound(new ResponseDto { IsSuccess = false, Message = "Rol no encontrado" });

      // si el rol tiene asignados usuarios no se puede eliminar
      var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

      if (usersInRole.Count > 0) return BadRequest(new ResponseDto { IsSuccess = false, Message = "El rol tiene usuarios asignados, no se puede eliminar" });

      var roleResult = await _roleManager.DeleteAsync(role);

      if (!roleResult.Succeeded) return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error al eliminar el rol" });

      return Ok(new ResponseDto { IsSuccess = true, Message = "Rol eliminado exitosamente" });
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(RoleAssignDto roleAssignDto)
    {
      var user = await _userManager.FindByIdAsync(roleAssignDto.UserId);

      if (user == null) return NotFound(new ResponseDto { IsSuccess = false, Message = "Usuario no encontrado" });

      var role = await _roleManager.FindByIdAsync(roleAssignDto.RoleId);

      if (role == null) return NotFound(new ResponseDto { IsSuccess = false, Message = "Rol no encontrado" });

      var userRoles = await _userManager.GetRolesAsync(user);

      if (userRoles.Contains(role.Name!)) return BadRequest(new ResponseDto { IsSuccess = false, Message = "El usuario ya tiene el rol asignado" });

      var result = await _userManager.AddToRoleAsync(user, role.Name!);

      if (!result.Succeeded) return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error al asignar el rol" });

      return Ok(new ResponseDto { IsSuccess = true, Message = "Rol asignado exitosamente" });
    }

    [HttpPost("removeRole")]
    public async Task<IActionResult> RemoveRole(RoleAssignDto roleAssignDto)
    {
      var user = await _userManager.FindByIdAsync(roleAssignDto.UserId);

      if (user == null) return NotFound(new ResponseDto { IsSuccess = false, Message = "Usuario no encontrado" });

      var role = await _roleManager.FindByIdAsync(roleAssignDto.RoleId);

      if (role == null) return NotFound(new ResponseDto { IsSuccess = false, Message = "Rol no encontrado" });

      var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

      if (!result.Succeeded) return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error al remover el rol" });

      return Ok(new ResponseDto { IsSuccess = true, Message = "Rol removido exitosamente" });
    }

  }
}

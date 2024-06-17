using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
  public class RoleDto
  {
    [Required(ErrorMessage = "El nombre del rol es requerido")]
    public required string RoleName { get; set; }
  }
}

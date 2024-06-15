using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
  public class RegisterDto
  {
    [Required]
    [EmailAddress]
    public required string Correo { get; set; }

    [Required]
    public required string Nombres { get; set; }

    [Required]
    public required string ApellidoPaterno { get; set; }

    [Required]
    public required string ApellidoMaterno { get; set; }

    [Required]
    public required string Password { get; set; }

    public string[]? Roles { get; set; }
  }
}

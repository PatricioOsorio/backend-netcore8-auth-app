using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
  public class LoginDto
  {
    [Required]
    [EmailAddress]
    public required string Correo { get; set; }

    [Required]
    public required string Password { get; set; }

  }
}

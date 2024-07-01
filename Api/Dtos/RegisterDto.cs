using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
  public class RegisterDto
  {
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Names { get; set; }

    [Required]
    public required string PaternalLastName { get; set; }

    [Required]
    public required string MothersLastName { get; set; }

    [Required]
    public required string Password { get; set; }

    public string[]? Roles { get; set; }
  }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
  public class ForgotPasswordDto
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}

namespace Api.Dtos
{
  public class UserDetailDto
  {
    public string? Id { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Correo { get; set; }
    public string[]? Roles { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsDoubleFactoEnabled { get; set; }
    public bool? IsPhoneNumberConfirmed{ get; set; }
    public int? AccessFailedCount { get; set; }
  }
}

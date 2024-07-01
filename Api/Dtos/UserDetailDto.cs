namespace Api.Dtos
{
  public class UserDetailDto
  {
    public string? Id { get; set; }
    public string? Names { get; set; }
    public string? PaternalLastName { get; set; }
    public string? MothersLastName { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsDoubleFactoEnabled { get; set; }
    public bool? IsPhoneNumberConfirmed { get; set; }
    public int? AccessFailedCount { get; set; }
  }
}

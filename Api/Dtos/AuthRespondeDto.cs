namespace Api.Dtos
{
  public class AuthRespondeDto
  {
    public string? Token { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
  }
}

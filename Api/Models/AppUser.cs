using Microsoft.AspNetCore.Identity;

namespace Api.Models
{
  public class AppUser : IdentityUser
  {
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
    }
}

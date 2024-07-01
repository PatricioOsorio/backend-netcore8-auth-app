using Microsoft.AspNetCore.Identity;

namespace Api.Models
{
  public class AppUser : IdentityUser
  {
        public string Names { get; set; }
        public string PaternalLastName { get; set; }
        public string MothersLastName { get; set; }
    }
}

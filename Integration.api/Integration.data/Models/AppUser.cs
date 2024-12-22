using Microsoft.AspNetCore.Identity;
namespace Integration.data.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Custom { get; set; }
    }
}

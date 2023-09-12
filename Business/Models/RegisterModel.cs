using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class RegisterModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? EmailAddress { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}

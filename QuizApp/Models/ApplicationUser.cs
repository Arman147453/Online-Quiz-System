using Microsoft.AspNetCore.Identity;

namespace QuizApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}

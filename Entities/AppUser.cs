using Microsoft.AspNetCore.Identity;

namespace SurveyApp.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public List<Survey>? Surveys { get; set; }
        public Facility? Facility { get; set; }
    }
}

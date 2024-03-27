using SurveyApp.Entities;

namespace SurveyApp.Models
{
    public class FacilityAddUserModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<AppUser>? Users { get; set; }
        public List<AppUser>? FacilityUsers { get; set; }
    }
}

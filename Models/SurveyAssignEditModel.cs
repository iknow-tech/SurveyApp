using SurveyApp.Entities;

namespace SurveyApp.Models
{
    public class SurveyAssignEditModel
    {
        public string? Id { get; set; }
        public AppUser? AppUser { get; set; }
        public List<Survey>? Surveys { get; set; }
        public List<Survey>? UserSurveys { get; set; }
    }
}

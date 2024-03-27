using SurveyApp.Entities;

namespace SurveyApp.Models
{
    public class SurveyAssignedModel
    {
        public Survey? Survey { get; set; }

        public List<AppUser>? AppUsers { get; set; }

        public Dictionary<int, bool>? QuestionAnswers { get; set; }
        public Dictionary<int, string>? QuestionPhotos { get; set; }
        public Dictionary<int, string>? QuestionUsers { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
    }
}

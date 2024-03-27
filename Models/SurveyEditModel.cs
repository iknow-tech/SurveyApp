using SurveyApp.Entities;

namespace SurveyApp.Models
{
    public class SurveyEditModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Question>? Questions { get; set; }
        public List<Question>? SurveyQuestions { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Models
{
    public class QuestionCreateModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}

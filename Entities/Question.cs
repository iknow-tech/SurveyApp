namespace SurveyApp.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Survey? Survey { get; set; }
        public List<Answer>? Answers { get; set; }
    }
}

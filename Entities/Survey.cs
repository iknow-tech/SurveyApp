namespace SurveyApp.Entities
{
    public class Survey
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Question>? Questions { get; set; }
        public List<AppUser>? AppUsers { get; set; }
    }
}

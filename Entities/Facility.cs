namespace SurveyApp.Entities
{
    public class Facility
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Local { get; set; }
        public List<AppUser>? AppUsers { get; set; }
    }
}

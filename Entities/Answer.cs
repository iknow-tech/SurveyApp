namespace SurveyApp.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        public bool isSuccess { get; set; }
        public AppUser? AppUser { get; set; }
        public Question? Question { get; set; }
        public string? Photo { get; set; }
        public string? Description { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}

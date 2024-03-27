namespace SurveyApp.Entities
{
    public class SurveyStatus
    {
        public int Id { get; set; }
        public Survey? Survey { get; set; }
        public Facility? Facility { get; set; }
        public AppUser? AppUser { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
    }
}

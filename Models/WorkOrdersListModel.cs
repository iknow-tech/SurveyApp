using SurveyApp.Entities;

namespace SurveyApp.Models
{
    public class WorkOrdersListModel
    {
        public List<Survey>? Surveys { get; set; }
        public List<AppUser>? AppUsers { get; set; }
    }
}

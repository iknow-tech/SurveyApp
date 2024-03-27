using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Models
{
    public class UserEditModel
    {
        public string? Id { get; set; }

        public string? UserName { get; set; }

        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        //public IList<string>? SelectedRoles { get; set; }
    }
}

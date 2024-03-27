using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Models
{
    public class AccountLoginModel
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; } = true;
    }
}

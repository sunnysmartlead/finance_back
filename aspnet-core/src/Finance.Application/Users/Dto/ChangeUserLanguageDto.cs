using System.ComponentModel.DataAnnotations;

namespace Finance.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
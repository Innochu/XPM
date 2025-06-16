using System.ComponentModel.DataAnnotations;

namespace XPMTest.Application.Dtos
{
    public class AppUserLoginDto
    {
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}

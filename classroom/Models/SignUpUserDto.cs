using System.ComponentModel.DataAnnotations;

namespace classroom.Models
{
    public class SignUpUserDto
    {
        [Required]
        public string? UserName { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }

        [Required]
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace aboutus.Models
{
    public class Sing_In
    {
        [Required(ErrorMessage ="Email 必須輸入")]
        public string Email { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Passeword 必須輸入")]
        [StringLength(20, MinimumLength = 6, ErrorMessage ="6-20字元")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }



    }
}

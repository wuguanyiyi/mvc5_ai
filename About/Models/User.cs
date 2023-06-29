using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace About.Models
{
    public class User
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="必須輸入")]
        public string Name { get; set; }
        [Required(ErrorMessage = "必須輸入")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "必須輸入")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "必須輸入")]
        [DataType(DataType.Password)]
        public string PasswordConfirmed { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace English_Trainer
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Can not be empty")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9-_]+$", ErrorMessage = "The username must contain only letters of the Latin alphabet (in any case), numbers, and hyphens/underscores\nThe first symbol must be a letter")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Minimum length is 2 symbols")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Can not be empty")]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Minimum length is 4 symbols")]
        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*]+$", ErrorMessage = "The password must contain only letters of the Latin alphabet (in any case), numbers, and symbols !@#$%^&*")]
        public string Password { get; set; }
    }
}

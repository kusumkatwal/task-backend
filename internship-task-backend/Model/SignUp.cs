using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace internship_task_backend.Model
{
    public class SignUp
    {
        public class SignUpRequest
        {
            [Required]
            public string email { get; set; }

            [Required]
            public string password { get; set; }

            [Required]
            public string confirm_password { get; set; }
            

        }
        public class SignUpResponse
        {
            public bool success { get; set; }
            public string message { get; set; }

        }
    }
}

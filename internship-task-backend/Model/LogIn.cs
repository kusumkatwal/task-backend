using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace internship_task_backend.Model
{
    public class LogIn
    {
        public class LogInRequest
        {
            [Required]
            public string? email { get; set; }
            [Required]
            public string? password { get; set; }
        }
        public class LogInResponse
        {
            public bool success { get; set; }
            public string? message { get; set; }
            public UserLoginInfo? loginInfo { get; set; }

            public string? token { get; set; }
        }
        public class UserLoginInfo
        {
            public string? email { get; set; }
            public string? password { get; set; }
        }
    }
}

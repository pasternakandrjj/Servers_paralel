using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Servers_paralel.Models
{
    public class LoginModel
    {
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Enter valid email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password length > 5 && < 20")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Enter valid email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password length > 5 && < 20")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password are not equal!")]
        public string ConfirmPassword { get; set; }

        [Range(1, 100)]
        public int Age { get; set; }
    }
    public class Info
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Password length > 5 && < 20")]
        public string bytes { get; set; }

        [Required]
        public int WhichServer { get; set; }

        [Required]
        public bool IsDone { get; set; }

        [Required]
        public double Percentage { get; set; }

        [Required]
        public bool IsPaused { get; set; }
    }
}
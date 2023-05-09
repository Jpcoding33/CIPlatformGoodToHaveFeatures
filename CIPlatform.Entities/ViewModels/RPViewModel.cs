using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class RPViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,15}$", ErrorMessage = "Minimum 8 and maximum 15 characters, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string? Password { get; set; }

        [Required]
        [NotMapped]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
        public IEnumerable<Banner> BannerList { get; set; } = new List<Banner>();


    }
}

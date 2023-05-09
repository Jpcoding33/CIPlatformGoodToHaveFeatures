using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class UserViewModel
    {
        //User details
        [Required(ErrorMessage = "Required!")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? LastName { get; set; }
        public string? WhyIVolunteer { get; set; }
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long CityId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? ProfileText { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Title { get; set; }

        //change password fields
        [Required(ErrorMessage ="Required!")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,15}$", ErrorMessage = "Minimum 8 and maximum 15 characters, at least one uppercase letter, one lowercase letter, one number and one special character")] 
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage ="Required!")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = null!;
        public User? UserDetail { get; set; }
        public IEnumerable<Skill> SkillList { get; set; } = new List<Skill>();
        public IEnumerable<Country>? CountryList { get; set; } = new List<Country>();
        public IEnumerable<UserSkill>? UserSkillList { get; set; } = new List<UserSkill>();
    }
}

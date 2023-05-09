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
    public class RegistrationViewModel
    {

        [Required(ErrorMessage = "FirstName is required!")]
        //[StringLength(15, MinimumLength = 3)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required!")]
        //[StringLength(15, MinimumLength = 3)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required!")]
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public long PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select Country!")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Please Select City!")]
        public long CityId { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,15}$", ErrorMessage = "Minimum 8 characters and maximum 15, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
        public IEnumerable<Country> CountryList { get; set; } = new List<Country>();

        public IEnumerable<Banner> BannerList { get; set; } = new List<Banner>();

    }
}

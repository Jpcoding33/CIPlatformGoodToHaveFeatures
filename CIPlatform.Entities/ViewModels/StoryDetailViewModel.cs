using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class StoryDetailViewModel
    {
        public User? UserDetail { get; set; }
        public Story? StoryDetail { get; set; }
        public User? UserDetailOfThisStory { get; set; }
        public IEnumerable<Country>? CountryList { get; set; } = new List<Country>();
        public IEnumerable<City>? CityList { get; set; } = new List<City>();
        public IEnumerable<User>? UserList { get; set; } = new List<User>();
        public IEnumerable<StoryMedium>? StoryMediaList { get; set; } = new List<StoryMedium>();
    }
}

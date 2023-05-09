using CIPlatformWeb.Entities.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class StoryPageViewModel
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "Please Select Mission!")]
        public long MissionId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Description { get; set; }
        public long StoryId { get; set; }
        public string Type { get; set; } = null!;
        public string? YoutubeLink { get; set; }

        [Required(ErrorMessage = "Required!")]
        [RegularExpression("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube(-nocookie)?\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$", ErrorMessage = "Enter valid youtube link!")]
        public string VideoUrls { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        public DateTime DateCreated { get; set; }
        public User? UserDetail { get; set; }
        public int Views { get; set; }
        public IEnumerable<MissionApplication>? MissionApplicationList { get; set; } = new List<MissionApplication>();
        public IEnumerable<Story>? StoryList { get; set; } = new List<Story>();
        public IEnumerable<StoryMedium>? StoryMediaList { get; set; } = new List<StoryMedium>();
        public IEnumerable<MissionTheme>? MissionThemeList { get; set; } = new List<MissionTheme>();
        public IEnumerable<User>? UsersList { get; set; } = new List<User>();
        public IEnumerable<Mission>? MissionList { get; set; } = new List<Mission>();
        public IEnumerable<City>? CityList { get; set; } = new List<City>();
    }
}

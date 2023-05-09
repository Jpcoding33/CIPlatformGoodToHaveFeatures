using CIPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformWeb.Entities.ViewModels
{
    public class LandingPageViewModel
    {
        public User? UserDetail { get; set; } = null!;
        public IEnumerable<Country> CountryList { get; set; } = new List<Country>();
        public IEnumerable<MissionTheme> MissionThemeList { get; set; } = new List<MissionTheme>();    
        public IEnumerable<City> CityList { get; set; } = new List<City>();
        public IEnumerable<Skill> SkillList { get; set; } = new List<Skill>();
        public IEnumerable<FavoriteMission> FavMissionList { get; set; } = new List<FavoriteMission>();
        public IEnumerable<Mission> MissionList { get; set; } = new List<Mission>();
        public IEnumerable<MissionRating> MissionRatings { get; set; } = new List<MissionRating>();
        public IEnumerable<MissionApplication> MissionApplicationList { get; set; } = new List<MissionApplication>();
        public IEnumerable<User> UsersList { get; set; } = new List<User>();
        public IEnumerable<GoalMission> TotalGoalList { get;set; } = new List<GoalMission>();
        public IEnumerable<Timesheet> AchievedGoalList { get; set; } = new List<Timesheet>();
    }
}

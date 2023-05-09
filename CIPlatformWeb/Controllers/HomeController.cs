using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using CIPlatformWeb.Repositories.GenericRepository.Interface;

namespace CIPlatformWeb.Controllers
{
    public class HomeController : CommonController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _uow;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow) : base(uow)
        {
            _logger = logger;
            _uow = uow;
        }

        //PlatformID landing page 
        public IActionResult Index()
        {
            IEnumerable<Skill> skillList = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null);
            LandingPageViewModel obj = new()
            {
                UserDetail = GetThisUser(),
                CountryList = _uow.Country.GetAll(),
                CityList = _uow.City.GetAll(),
                MissionThemeList = _uow.MissionTheme.GetAccToFilter(theme => theme.DeletedAt == null),
                SkillList = skillList,
            };
            return View(obj);
        }

        //landing page cards 
        public IActionResult CardData(string[]? country, string[]? city, string[]? theme, string[]? skill, string? sortval, string? exploreval, string? searchStr, int pg = 1)
        {
            User? user = GetThisUser();
            IEnumerable<User> userList = new List<User>();
            IEnumerable<MissionApplication> missAppList = _uow.MissionApplication.GetAll();
            if (user != null)
            {
                userList = _uow.User.GetAccToFilter(m => m.UserId != user.UserId);
            }
            IEnumerable<Skill> skills = _uow.Skill.GetAll();
            IEnumerable<Mission> missList = GetMissions(searchStr, country, city, theme, skill, sortval, exploreval);
            IEnumerable<FavoriteMission> favMissionList = _uow.FavoriteMission.GetAll();
            IEnumerable<MissionRating> missRatings = _uow.MissionRating.GetAll();
            IEnumerable<Timesheet> achievedGoalList = _uow.TimeSheet.GetAccToFilter(timesheet => timesheet.Mission.MissionType == "Goal" && timesheet.DeletedAt == null);
            IEnumerable<GoalMission> totalGoalList = _uow.GoalMisison.GetAll();
            LandingPageViewModel obj = new()
            {
                SkillList = skills,
                UserDetail = user,
                MissionList = missList,
                FavMissionList = favMissionList,
                MissionRatings = missRatings,
                UsersList = userList,
                MissionApplicationList = missAppList,
                TotalGoalList = totalGoalList,
                AchievedGoalList = achievedGoalList,
            };
            
            int resCount = obj.MissionList.Count();
            const int pageSize = 9;
            if (pg < 1)
                pg = 1;
            var pager = new PaginationViewModel(resCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            obj.MissionList = obj.MissionList.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.userPager = pager;

            ViewBag.missionCount = resCount;

            return PartialView("_PartialViewForMissionCards", obj);
        }


        //Get missions

        public IEnumerable<Mission> GetMissions(string? search, string[]? countries, string[]? cities, string[]? themes, string[]? skills, string? sortval, string? exploreval)
        {
            User user = GetThisUser();
            IEnumerable<Mission> missions = _uow.Mission.GetMission(mission => mission.DeletedAt == null);


            //explore menu 
            switch (exploreval)
            {
                case "topthemes":
                    missions = missions.GroupBy(mission => mission.MissionThemeId).OrderByDescending(group => group.Count()).First().ToList().Take(9);
                    break;
                case "mostranked":
                    missions = missions.Where(mission => mission.MissionRatings.Any()).OrderByDescending(mission => mission.MissionRatings.Average(rating => rating.Rating)).Take(9);
                    break;
                case "topfav":
                    missions = missions.OrderByDescending(mission => mission.FavoriteMissions.Count()).ToList();
                    break;
                case "random":
                    var random = new Random();
                    missions = missions.OrderBy(mission => random.Next()).Take(3);
                    break;
            }

            //search mission by searchbar
            if (search != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Contains(search.ToLower())).ToList();
            }

            //filter by country, city, theme and 
            if (countries?.Length > 0 || cities?.Length > 0 || themes?.Length > 0 || skills?.Length > 0)
            {
                if (countries?.Length > 0)
                {
                    missions = missions.Where(m => countries.Contains(m.Country.Name)).ToList();
                }

                else if (cities?.Length > 0)
                {
                    missions = missions.Where(m => cities.Contains(m.City.Name)).ToList();
                }

                else if (themes?.Length > 0)
                {
                    missions = missions.Where(m => themes.Contains(m.MissionTheme.Title)).ToList();
                }
                else if (skills?.Length > 0)
                {
                    missions = missions.Where(mission => mission.MissionSkills.Any(skill => skills.Contains(skill.Skill.SkillName))).ToList();
                }
            }            

            switch (sortval)
            {
                case "new":
                    missions = missions.OrderByDescending(m => m.StartDate).ToList();
                    break;
                case "old":
                    missions = missions.OrderBy(m => m.StartDate).ToList();
                    break;
                case "low":
                    missions = missions.Where(mission => mission.MissionType == "Time").OrderBy(mission => mission.TotalSeats).ToList();
                    break;
                case "high":
                    missions = missions.Where(mission => mission.MissionType == "Time").OrderByDescending(mission => mission.TotalSeats).ToList();
                    break;
                case "fav":
                    missions = missions.Where(mission => mission.FavoriteMissions.Any(favMiss => favMiss.UserId == user.UserId)).ToList();
                    break;
                case "deadline":
                    missions = missions.Where(mission => mission.MissionType == "Time").OrderByDescending(mission => mission.MissionDeadline).ToList();
                    break;
            }

            return missions;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CIPlatformWeb.Controllers
{
    public class VolPageController : Controller
    {
        private readonly IUnitOfWork _uow;
        public VolPageController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public User GetThisUser()
        {
            ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
            string? email = identity?.FindFirst(ClaimTypes.Email)?.Value;
            User? user = _uow.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //Volunteering mission page 
        public IActionResult VolunteeringMissionPage(int? id)
        {
            User? user = GetThisUser();
            Mission? thisMission = _uow.Mission.GetThisMission(m => m.MissionId == id);
            IEnumerable<Mission> relatedMissions = _uow.Mission.GetMission(m => m.MissionId != id && m.DeletedAt == null && m.Status == 1 && (m.CityId == thisMission.CityId || m.CountryId == thisMission.CountryId || m.MissionThemeId == thisMission.MissionThemeId)).Take(3);
            IEnumerable<User> userList = _uow.User.GetAll();
            IEnumerable<Timesheet> achievedGoalList = _uow.TimeSheet.GetAccToFilter(timesheet => timesheet.Mission.MissionType == "Goal" && timesheet.DeletedAt == null);
            IEnumerable<GoalMission> totalGoalList = _uow.GoalMisison.GetAll();
            MissionRating UserRatingForThisMission = new MissionRating();

            if (user != null)
            {
                UserRatingForThisMission = _uow.MissionRating.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == id);
            }
            IEnumerable<MissionRating> missionRatingList = _uow.MissionRating.GetAll();
            IEnumerable<City> cities = _uow.City.GetAll();
            IEnumerable<Country> countries = _uow.Country.GetAll();
            IEnumerable<MissionTheme> themes = _uow.MissionTheme.GetAll();
            IEnumerable<FavoriteMission> favMissions = _uow.FavoriteMission.GetAll();
            IEnumerable<Comment> comments = _uow.Comment.GetAccToFilter(m => m.MissionId == id).OrderByDescending(m => m.CreatedAt);
            IEnumerable<MissionApplication> missAppList = _uow.MissionApplication.GetMissAppList();
            IEnumerable<MissionApplication> resVol = _uow.MissionApplication.GetAccToFilter(m => m.MissionId == id && m.ApprovalStatus == "APPROVED");

            VolunteeringPageViewModel obj = new()
            {
                UsersList = userList,
                CityList = cities,
                CountryList = countries,
                MissionThemeList = themes,
                MissionDetail = thisMission,
                UserDetail = user,
                RelatedMissionList = relatedMissions,
                FavMissionList = favMissions,
                CommentList = comments,
                UserRating = UserRatingForThisMission,
                MissionRatings = missionRatingList,
                MissionApplicationList = missAppList,
                RecentVolunteerList = resVol,
                TotalGoalList = totalGoalList,
                AchievedGoalList = achievedGoalList,
            };
            return View(obj);
        }

        //Rating by user
        public void changeRatingByUser(int rating, int missionId)
        {
            User user = GetThisUser();
            MissionRating UserRatingForThisMission = _uow.MissionRating.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == missionId);
            if (UserRatingForThisMission == null)
            {
                MissionRating obj = new()
                {
                    Rating = rating,
                    MissionId = missionId,
                    UserId = user.UserId,
                };
                _uow.MissionRating.Add(obj);
                _uow.Save();
            }
            else
            {
                UserRatingForThisMission.Rating = rating;
                UserRatingForThisMission.UpdatedAt = DateTime.Now;
                _uow.MissionRating.Update(UserRatingForThisMission);
                _uow.Save();
            }
        }

        //Mission application
        public void MissionApplication(int missId)
        {
            User user = GetThisUser();
            MissionApplication thisMissApp = _uow.MissionApplication.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == missId);
            if (thisMissApp != null && thisMissApp.ApprovalStatus == "DECLINE")
            {
                thisMissApp.ApprovalStatus = "PENDING";
                thisMissApp.AppliedAt = DateTime.Now;
                thisMissApp.UpdatedAt = DateTime.Now;
                _uow.MissionApplication.Update(thisMissApp);
                _uow.Save();
            }
            else
            {
                MissionApplication obj = new()
                {
                    UserId = user.UserId,
                    MissionId = missId,
                    AppliedAt = DateTime.Now,
                };
                _uow.MissionApplication.Add(obj);
                _uow.Save();
            }
        }

        //Add comment function 
        public IActionResult addComment(int? missId, string? commentText)
        {
            User user = GetThisUser();
            Comment obj = new()
            {
                CommentText = commentText,
                UserId = user.UserId,
                MissionId = (long)missId,
            };
            _uow.Comment.Add(obj);
            _uow.Save();

            return RedirectToAction("VolunteeringMissionPage", new { id = missId });
        }
    }
}

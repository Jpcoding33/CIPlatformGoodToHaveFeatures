using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using CIPlatformWeb.Entities.DataModels;
using System.Security.Claims;
using CIPlatformWeb.Entities.ViewModels;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CIPlatformWeb.Controllers
{
    public class UserController : CommonController
    {
        private readonly IUnitOfWork _uow;
        public UserController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        //user profile page
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            User user = GetThisUser();
            IEnumerable<Skill> skillList = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null);
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            IEnumerable<UserSkill> userSkillList = _uow.UserSkill.GetAccToFilter(userSkill => userSkill.UserId == user.UserId);
            UserViewModel obj = new()
            {
                UserDetail = user,
                SkillList = skillList,
                UserSkillList = userSkillList,
                CountryList = countryList,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Title = user.Title,
                ProfileText = user.ProfileText,
                CountryId = user.CountryId,
                CityId = user.CityId,
                WhyIVolunteer = user.WhyIVolunteer,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                LinkedInUrl = user.LinkedInUrl,
            };
            return View(obj);
        }

        //update profile pic
        [HttpPost]
        public IActionResult UpdateProfilePic(IFormFile profile_Pic)
        {
            User? user = GetThisUser();

            if (user != null && profile_Pic != null)
            {
                string imgExt = Path.GetExtension(profile_Pic.FileName);
                if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                {
                    if (user.Avatar != null)
                    {
                        string fileName = Path.Combine(user.UserId + profile_Pic.FileName);
                        string imagePathForDb = Path.Combine("/ProfilePics/" + fileName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imagePathForDb);
                        if (!imagePathForDb.Equals(user.Avatar))
                        {
                            string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + user.Avatar);
                            if (System.IO.File.Exists(alrExists))
                            {
                                System.IO.File.Delete(alrExists);
                            }

                            using (FileStream stream = new(finalPath, FileMode.Create))
                            {
                                profile_Pic.CopyTo(stream);
                            }
                            user.Avatar = imagePathForDb;
                            user.UpdatedAt = DateTime.Now;
                            _uow.User.Update(user);
                        }
                    }
                    else
                    {
                        string fileName = Path.Combine(user.UserId + profile_Pic.FileName);
                        string imagePathForDb = Path.Combine("/ProfilePics/" + fileName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imagePathForDb);
                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            profile_Pic.CopyTo(stream);
                        }
                        user.Avatar = imagePathForDb;
                        user.UpdatedAt = DateTime.Now;
                        _uow.User.Update(user);
                    }
                }
            }
            _uow.Save();
            return RedirectToAction("Profile");
        }

        //change password 
        [HttpPost]
        public IActionResult ChangePassword(UserViewModel obj)
        {
            User? user = GetThisUser();
            if (user != null && obj.OldPassword != null && obj.NewPassword != null)
            {
                if (BCrypt.Net.BCrypt.Verify(obj.OldPassword, user.Password))
                {
                    string newPass = BCrypt.Net.BCrypt.HashPassword(obj.NewPassword);

                    user.Password = newPass;
                    user.UpdatedAt = DateTime.Now;
                    _uow.User.Update(user);
                    _uow.Save();

                    return Json(1);
                }
            }

            return Json(0);
        }

        //get city according to selected country
        public JsonResult CascadeCity(long countryId)
        {
            User user = GetThisUser();
            var cityId = user.CityId;
            IEnumerable<City> cityList = _uow.City.GetAccToFilter(city => city.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }

        //update user details
        [HttpPost]
        public IActionResult UpdateUser(UserViewModel obj, List<long> finalSkillList)
        {
            User? user = GetThisUser();
            var userSkillsId = _uow.UserSkill.GetAccToFilter(userSkill => userSkill.UserId == user.UserId).Select(m => m.SkillId);
            if (user != null)
            {
                user.FirstName = obj.FirstName;
                user.LastName = obj.LastName;
                user.ProfileText = obj.ProfileText;
                user.CityId = obj.CityId;
                user.CountryId = obj.CountryId;
                user.EmployeeId = obj.EmployeeId;
                user.Department = obj.Department;
                user.Title = obj.Title;
                user.WhyIVolunteer = obj.WhyIVolunteer;
                user.LinkedInUrl = obj.LinkedInUrl;
                user.UpdatedAt = DateTime.Now;

                _uow.User.Update(user);

                if (finalSkillList.Count == 0)
                {
                    foreach (var item in userSkillsId)
                    {
                        UserSkill deleteSkill = _uow.UserSkill.GetFirstOrDefault(userSkill => userSkill.SkillId == item);
                        _uow.UserSkill.Remove(deleteSkill);
                    }
                }
                else
                {
                    var skillToBeAdded = finalSkillList.Except(userSkillsId);
                    foreach (var skillId in skillToBeAdded)
                    {
                        UserSkill uSkills = new()
                        {
                            UserId = user.UserId,
                            SkillId = skillId,
                        };
                        _uow.UserSkill.Add(uSkills);
                    }

                    var skillToBeDeleted = userSkillsId.Except(finalSkillList);
                    foreach (var skillid in skillToBeDeleted)
                    {
                        UserSkill deleteSkill = _uow.UserSkill.GetFirstOrDefault(userSkill => userSkill.SkillId == skillid);
                        _uow.UserSkill.Remove(deleteSkill);
                    }
                }
                _uow.Save();
            }

            return RedirectToAction("Profile");
        }


        //volunteering timesheet
        [Authorize]
        public IActionResult VolTimeSheet()
        {
            User? user = GetThisUser();
            IEnumerable<MissionApplication> timeBasedMissionAppList = _uow.MissionApplication.GetMissionApplicationList(mission => mission.UserId == user.UserId && mission.ApprovalStatus == "APPROVED" && mission.Mission.MissionType == "Time");
            IEnumerable<MissionApplication> TimeBasedMissionAppList = _uow.MissionApplication.GetMissionApplicationsData(user.UserId);
            IEnumerable<MissionApplication> goalBasedMissionAppList = _uow.MissionApplication.GetMissionApplicationList(mission => mission.UserId == user.UserId && mission.ApprovalStatus == "APPROVED" && mission.Mission.MissionType == "Goal");
            IEnumerable<Timesheet> timeBasedData = _uow.TimeSheet.GetTimeSheetData(timeData => timeData.UserId == user.UserId && timeData.Mission.MissionType == "Time" && timeData.DeletedAt == null);
            IEnumerable<Timesheet> goalBasedData = _uow.TimeSheet.GetTimeSheetData(goalData => goalData.UserId == user.UserId && goalData.Mission.MissionType == "Goal" && goalData.DeletedAt == null);
            VolTimeSheetViewModel obj = new();
            obj.UserDetail = user;
            obj.TimeBasedMissionAppList = timeBasedMissionAppList;
            obj.GoalBasedMissionAppList = goalBasedMissionAppList;
            obj.TimeBasedData = timeBasedData;
            obj.GoalBasedData = goalBasedData;
            return View(obj);
        }

        //add and update timesheet
        [HttpPost]
        public IActionResult AddUpdateTimesheet(VolTimeSheetViewModel obj)
        {
            User? user = GetThisUser();
            var hrs = obj.Hours;
            var min = obj.Minutes;
            TimeSpan time = new(hrs, min, 0);
            if (obj.TimeSheetId == 0)
            {
                Timesheet data = new()
                {
                    UserId = user.UserId,
                    MissionId = obj.MissionId,
                    TotalTime = time,
                    Action = obj.Action,
                    DateVolunteered = obj.DateVolunteered,
                    Notes = obj.Notes,
                    Status = "SUBMIT_FOR_APPROVAL",
                };
                _uow.TimeSheet.Add(data);

                GoalMission goalMissionData = _uow.GoalMisison.GetFirstOrDefault(goalmission => goalmission.MissionId == obj.MissionId);
                if(obj.Action > 0)
                {
                    goalMissionData.AchievedValue += obj.Action;
                    _uow.GoalMisison.Update(goalMissionData);
                }
                
                _uow.Save();
                TempData["success"] = "Data added successfully!";
                return RedirectToAction("VolTimeSheet");
            }
            else
            {                
                Timesheet dataToBeUpdated = _uow.TimeSheet.GetFirstOrDefault(timeSheetData => timeSheetData.TimesheetId == obj.TimeSheetId);
                GoalMission goalMissionData = _uow.GoalMisison.GetFirstOrDefault(goalmission => goalmission.MissionId == obj.MissionId);
                if (obj.Action > 0)
                {
                    goalMissionData.AchievedValue -= dataToBeUpdated.Action;
                    goalMissionData.AchievedValue += obj.Action;
                    _uow.GoalMisison.Update(goalMissionData);
                }

                if (dataToBeUpdated != null)
                {
                    dataToBeUpdated.MissionId = obj.MissionId;
                    dataToBeUpdated.Action = obj.Action;
                    dataToBeUpdated.TotalTime = time;
                    dataToBeUpdated.Notes = obj.Notes;
                    dataToBeUpdated.DateVolunteered = obj.DateVolunteered;
                    dataToBeUpdated.UpdatedAt = DateTime.Now;

                    _uow.TimeSheet.Update(dataToBeUpdated);
                    _uow.Save();

                    TempData["success"] = "Data updated successfully!";
                    return RedirectToAction("VolTimeSheet");
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("VolTimeSheet");
        }

        public IActionResult GetTimeSheetData(long tsId)
        {
            Timesheet tSData = _uow.TimeSheet.GetFirstOrDefault(data => data.TimesheetId == tsId);
            return Json(tSData);
        }

        [HttpPost]
        public IActionResult DeleteTimeSheetData(long tsId)
        {
            if (tsId > 0)
            {
                Timesheet dataToBeDel = _uow.TimeSheet.GetFirstOrDefault(data => data.TimesheetId == tsId);
                dataToBeDel.DeletedAt = DateTime.Now;
                _uow.TimeSheet.Update(dataToBeDel);
                _uow.Save();

                return RedirectToAction("VolTimeSheet");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("VolTimeSheet");
        }

        //logout
        public IActionResult Logout()
        {
            //var emailId = HttpContext.Session.GetString("emailId");
            //var user = _uow.Users.Where(m => m.Email == emailId).FirstOrDefault();
            //user.Status = 0;
            //_uow.SaveChanges();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            TempData["success"] = "Logged Out successfully";
            return RedirectToAction("Index", "Home");
        }
    }
}

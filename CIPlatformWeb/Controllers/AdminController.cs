using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using System.Reflection;
using System.Security.Cryptography;

namespace CIPlatformWeb.Controllers
{
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public class AdminController : CommonController
    {
        private readonly IUnitOfWork _uow;
        public AdminController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }


        //User lists get method
        [HttpGet]
        public IActionResult UserDetail()
        {
            IEnumerable<User> userLists = _uow.User.GetAccToFilter(user => user.DeletedAt == null);
            UserDetails obj = new()
            {
                UserLists = userLists
            };
            return View(obj);
        }

        //add user get method
        [HttpGet]
        public IActionResult AddUser()
        {
            User user = GetThisUser();
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            UserDetails obj = new();
            obj.thisUser = user;
            obj.CountryList = countryList;
            return View(obj);
        }

        //add user post method
        [HttpPost]
        public IActionResult AddUser(UserDetails obj, IFormFile userAvatar)
        {
            User status = _uow.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower());
            if (status == null)
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(obj.Password);
                User user = new()
                {
                    Role = obj.Role,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    PhoneNumber = obj.PhoneNumber,
                    Email = obj.Email,
                    Password = passwordHash,
                    CountryId = obj.CountryId,
                    CityId = obj.CityId,
                    EmployeeId = obj.EmployeeId,
                    Department = obj.Department,
                    Status = obj.Status,
                };
                _uow.User.Add(user);
                _uow.Save();

                if (userAvatar != null)
                {
                    User userForAvatar = _uow.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower());
                    string imgExt = Path.GetExtension(userAvatar.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string fileName = Path.Combine(userForAvatar.UserId + userAvatar.FileName);
                        string imagePathForDb = Path.Combine("/ProfilePics/" + fileName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imagePathForDb);
                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            userAvatar.CopyTo(stream);
                        }
                        userForAvatar.Avatar = imagePathForDb;
                        _uow.User.Update(user);
                        _uow.Save();
                    }
                }
                TempData["success"] = "User added successfully!";
                return RedirectToAction("UserDetail");

            }
            TempData["error"] = "Email Already Exists!";
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            obj.CountryList = countryList;
            return View(obj);
        }


        //cascade city for add user
        public JsonResult CascadeCity(long countryId)
        {
            IEnumerable<City> cityList = _uow.City.GetAccToFilter(cities => cities.CountryId == countryId);
            return new JsonResult(cityList);
        }

        //cascade city for Edit page get method
        public JsonResult CascadeCityForEdit(long countryId, int userId)
        {
            User user = new();
            long cityId = 0;
            if (userId > 0)
            {
                user = _uow.User.GetFirstOrDefault(user => user.UserId == userId);
                if (user != null)
                {
                    cityId = user.CityId;
                }
            }
            IEnumerable<City> cityList = _uow.City.GetAccToFilter(city => city.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }

        //Edit user get method
        [HttpGet]
        public IActionResult EditUser(long uid)
        {
            User userDetail = GetThisUser();
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            UserDetails obj = new();
            if (uid > 0)
            {
                User user = _uow.User.GetFirstOrDefault(user => user.UserId == uid);
                if (user != null)
                {
                    obj.Role = user.Role;
                    obj.UserId = user.UserId;
                    obj.Avatar = user.Avatar;
                    obj.FirstName = user.FirstName;
                    obj.LastName = user.LastName;
                    obj.PhoneNumber = user.PhoneNumber;
                    obj.Email = user.Email;
                    obj.EmployeeId = user.EmployeeId;
                    obj.Department = user.Department;
                    obj.CityId = user.CityId;
                    obj.CountryId = user.CountryId;
                    obj.EmployeeId = user.EmployeeId;
                    obj.Status = user.Status;
                    obj.CountryList = countryList;
                    obj.thisUser = userDetail;

                    return View(obj);
                }
            }
            return RedirectToAction("UserDetail");
        }

        //edit user post method
        [HttpPost]
        public IActionResult EditUser(UserDetails obj, IFormFile userAvatar)
        {
            if (obj.UserId > 0)
            {
                User user = _uow.User.GetFirstOrDefault(user => user.UserId == obj.UserId);
                if (user != null)
                {
                    user.FirstName = obj.FirstName;
                    user.LastName = obj.LastName;
                    user.PhoneNumber = obj.PhoneNumber;
                    user.Email = obj.Email;
                    user.EmployeeId = obj.EmployeeId;
                    user.Department = obj.Department;
                    user.CityId = obj.CityId;
                    user.CountryId = obj.CountryId;
                    user.EmployeeId = obj.EmployeeId;
                    user.Role = obj.Role;
                    user.ProfileText = obj.ProfileText;
                    user.Status = obj.Status;

                    if (userAvatar != null)
                    {
                        string imgExt = Path.GetExtension(userAvatar.FileName);
                        if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                        {
                            if (user.Avatar != null)
                            {
                                string fileName = Path.Combine(user.UserId + userAvatar.FileName);
                                string imagePathForDb = Path.Combine("/ProfilePics/" + fileName);
                                string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imagePathForDb);
                                if (!imagePathForDb.Equals(user.Avatar))
                                {
                                    string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + user.Avatar);
                                    if (user.Avatar != "\\images\\default-user-icon.png" && System.IO.File.Exists(alrExists))
                                    {
                                        System.IO.File.Delete(alrExists);
                                    }

                                    using (FileStream stream = new(finalPath, FileMode.Create))
                                    {
                                        userAvatar.CopyTo(stream);
                                    }
                                    user.Avatar = imagePathForDb;
                                }
                            }
                        }
                    }
                    user.UpdatedAt = DateTime.Now;
                    _uow.User.Update(user);
                    _uow.Save();
                    TempData["success"] = "User edited successfully!";
                    return RedirectToAction("UserDetail");
                }
            }
            TempData["error"] = "Something went wrong";
            return View(obj);
        }

        //delete user post method
        [HttpPost]
        public IActionResult DeleteUser(long uId)
        {
            User user = new();
            if (uId > 0)
            {
                user = _uow.User.GetFirstOrDefault(User => User.UserId == uId);
                if (user != null)
                {
                    user.DeletedAt= DateTime.Now;
                    user.Status = 0;
                    _uow.User.Update(user);
                    _uow.Save();
                    return RedirectToAction("UserDetail");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("UserDetail");
        }

        //CmsPage lists get method
        [HttpGet]
        public IActionResult CMSPage()
        {
            IEnumerable<CmsPage> cmsLists = _uow.CMSPage.GetAccToFilter(cms => cms.DeletedAt == null);
            CMSPageDetails obj = new()
            {
                CMSLists = cmsLists
            };
            return View(obj);
        }

        //add cms page get method
        [HttpGet]
        public IActionResult AddCMS()
        {
            return View();
        }
        //add cms page post method
        [HttpPost]
        public IActionResult AddCMS(CMSPageDetails obj)
        {
            if (obj != null)
            {
                CmsPage cmsPage = new()
                {
                    Title = obj.Title,
                    Description = obj.Description,
                    Slug = obj.Slug,
                    Status = obj.Status,
                };
                _uow.CMSPage.Add(cmsPage);
                _uow.Save();

                return RedirectToAction("CMSPage");
            }
            return View(obj);
        }

        //Edit cms page get method
        [HttpGet]
        public IActionResult EditCMS(long cmsId)
        {
            if (cmsId > 0)
            {
                CmsPage cmsDetail = _uow.CMSPage.GetFirstOrDefault(cmsdetail => cmsdetail.CmsPageId == cmsId);
                CMSPageDetails obj = new()
                {
                    CmsPageId = cmsDetail.CmsPageId,
                    Title = cmsDetail.Title,
                    Description = cmsDetail.Description,
                    Slug = cmsDetail.Slug,
                    Status = cmsDetail.Status,
                };
                return View(obj);
            }

            return RedirectToAction("CMSPage");
        }

        //edit cms page post method
        [HttpPost]
        public IActionResult EditCMS(CMSPageDetails obj)
        {
            if (obj != null)
            {
                CmsPage cmsDetail = _uow.CMSPage.GetFirstOrDefault(cmsdetail => cmsdetail.CmsPageId == obj.CmsPageId);
                if (cmsDetail != null)
                {
                    cmsDetail.CmsPageId = obj.CmsPageId;
                    cmsDetail.Title = obj.Title;
                    cmsDetail.Description = obj.Description;
                    cmsDetail.Slug = obj.Slug;
                    cmsDetail.Status = obj.Status;
                    cmsDetail.UpdatedAt = DateTime.Now;

                    _uow.CMSPage.Update(cmsDetail);
                    _uow.Save();

                    return RedirectToAction("CMSPage");
                }
            }

            return View(obj);
        }

        //delete cms post method
        [HttpPost]
        public IActionResult DeleteCMS(long cmsId)
        {
            CmsPage cms = new();
            if (cmsId > 0)
            {
                cms = _uow.CMSPage.GetFirstOrDefault(cms => cms.CmsPageId == cmsId);
                if (cms != null)
                {
                    cms.DeletedAt = DateTime.Now;
                    cms.Status = 0;
                    _uow.CMSPage.Update(cms);
                    _uow.Save();
                    return RedirectToAction("CMSPage");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("CMSPage");
        }

        //Mission lists get method
        [HttpGet]
        public IActionResult MissionDetails()
        {
            IEnumerable<Mission> missionLists = _uow.Mission.GetAccToFilter(mission => mission.DeletedAt == null);
            MissionDetails obj = new()
            {
                MissionLists = missionLists
            };
            return View(obj);
        }

        //add mission get method
        [HttpGet]
        public IActionResult AddMission()
        {
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            IEnumerable<MissionTheme> themeList = _uow.MissionTheme.GetAccToFilter(theme => theme.DeletedAt == null && theme.Status == 1);
            IEnumerable<Skill> skillList = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null && skill.Status == 1);
            MissionDetails obj = new()
            {
                CountryList = countryList,
                ThemeList = themeList,
                SkillList = skillList,
            };
            return View(obj);
        }

        //add mission post method
        [HttpPost]
        public IActionResult AddMission(MissionDetails obj, List<long> finalMissSkillList, List<IFormFile> MissionDocFiles, List<IFormFile> MissionImageFiles, string[] preloadedmissimage, string[] preloadedmissdocs)
        {
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            IEnumerable<MissionTheme> themeList = _uow.MissionTheme.GetAccToFilter(theme => theme.DeletedAt == null && theme.Status == 1);
            IEnumerable<Skill> skillList = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null && skill.Status == 1);
            if (obj != null)
            {
                Mission mission = new()
                {
                    Title = obj.Title,
                    ShortDescription = obj.ShortDescription,
                    Description = obj.Description,
                    CityId = obj.CityId,
                    CountryId = obj.CountryId,
                    OrganizationName = obj.OrganizationName,
                    OrganizationDetail = obj.OrganizationDetail,
                    StartDate = obj.StartDate,
                    EndDate = obj.EndDate,
                    MissionType = obj.MissionType,
                    MissionThemeId = obj.MissionThemeId,
                    TotalSeats = obj.TotalSeats,
                    MissionDeadline = obj.MissionDeadline,
                    Availability = obj.Availability,
                };

                _uow.Mission.Add(mission);
                _uow.Save();

                if(obj.MissionType == "Goal")
                {
                    GoalMission goalMiss = new()
                    {
                        MissionId = mission.MissionId,
                        GoalObjectiveText = obj.GoalObjectiveText,
                        GoalValue = obj.GoalValue,
                        AchievedValue = 0,
                    };

                    _uow.GoalMisison.Add(goalMiss);
                    _uow.Save();
                }

                long addedMissionId = mission.MissionId;
                IEnumerable<MissionMedium> missionMediaList = _uow.MissionMedia.GetAccToFilter(media => media.MissionId == addedMissionId);
                IEnumerable<MissionDocument> missionDocumentList = _uow.MissionDocuments.GetAccToFilter(media => media.MissionId == addedMissionId);
                SaveMissionMediaAndDocs(addedMissionId, obj.VideoUrl, missionMediaList, missionDocumentList, MissionDocFiles, MissionImageFiles, preloadedmissdocs, preloadedmissimage);
                var MissionSkillsId = _uow.MissionSkill.GetAccToFilter(missionSkill => missionSkill.MissionId == addedMissionId).Select(missionSkill => missionSkill.MissionId);

                AddMissionSkills(addedMissionId, MissionSkillsId, finalMissSkillList);
                
                return RedirectToAction("MissionDetails");
            }
            
            MissionDetails missDetails = new()
            {
                CountryList = countryList,
                ThemeList = themeList,
                SkillList = skillList,
            };
            return View(missDetails);
        }

        //cascade city for add mission get method
        public JsonResult AddMissCityCascade(long countryId)
        {
            IEnumerable<City> cityList = _uow.City.GetAccToFilter(cities => cities.CountryId == countryId);
            return new JsonResult(cityList);
        }

        //Edit mission get method
        [HttpGet]
        public IActionResult EditMission(long missionId)
        {
            if (missionId > 0)
            {
                Mission thisMissionDetails = _uow.Mission.GetFirstOrDefault(mission => mission.MissionId == missionId);
                GoalMission goalMission = _uow.GoalMisison.GetFirstOrDefault(goalMiss => goalMiss.MissionId == missionId);
                if (thisMissionDetails != null)
                {
                    IEnumerable<MissionSkill> missionSkillList = _uow.MissionSkill.GetAccToFilter(missionSkills => missionSkills.MissionId == missionId);
                    IEnumerable<MissionMedium> missionMediaList = _uow.MissionMedia.GetAccToFilter(missionMedia => missionMedia.MissionId == missionId);
                    IEnumerable<MissionDocument> missionDocList = _uow.MissionDocuments.GetAccToFilter(missionDoc => missionDoc.MissionId == missionId);
                    IEnumerable<Country> countryList = _uow.Country.GetAll();
                    IEnumerable<Skill> skillList = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null && skill.Status == 1 );
                    IEnumerable<MissionTheme> themeList = _uow.MissionTheme.GetAccToFilter(theme => theme.DeletedAt == null && theme.Status == 1);
                    MissionDetails obj = new()
                    {
                        MissionId = thisMissionDetails.MissionId,
                        Title = thisMissionDetails.Title,
                        ShortDescription = thisMissionDetails.ShortDescription,
                        Description = thisMissionDetails.Description,
                        CountryId = thisMissionDetails.CountryId,
                        CityId = thisMissionDetails.CityId,
                        OrganizationName = thisMissionDetails.OrganizationName,
                        OrganizationDetail = thisMissionDetails.OrganizationDetail,
                        StartDate = thisMissionDetails.StartDate,
                        EndDate = thisMissionDetails.EndDate,
                        MissionType = thisMissionDetails.MissionType,
                        TotalSeats = thisMissionDetails.TotalSeats,
                        MissionDeadline = thisMissionDetails.MissionDeadline,
                        MissionThemeId = thisMissionDetails.MissionThemeId,
                        Availability = thisMissionDetails.Availability,
                    };

                    if(goalMission != null)
                    {
                        obj.GoalObjectiveText = goalMission.GoalObjectiveText;
                        obj.GoalValue = goalMission.GoalValue;
                    }

                    obj.CountryList = countryList;
                    obj.ThemeList = themeList;
                    obj.SkillList = skillList;
                    obj.MissionSkillList = missionSkillList;
                    obj.MissionMediumList = missionMediaList;
                    obj.MissionDocumentList = missionDocList;

                    return View(obj);
                }
            }

            return RedirectToAction("MissionDetails");
        }

        //Edit mission post method 
        [HttpPost]
        public IActionResult EditMission(MissionDetails obj, List<long> finalMissSkillList, List<IFormFile> MissionDocFiles, List<IFormFile> MissionImageFiles, string[] preloadedmissimage, string[] preloadedmissdocs)
        {
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            IEnumerable<MissionTheme> themeList = _uow.MissionTheme.GetAccToFilter(theme => theme.DeletedAt == null && theme.Status == 1);
            IEnumerable<Skill> skillList = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null && skill.Status == 1);
            if (obj != null)
            {
                if(obj?.MissionId != null)
                {
                    Mission editThisMission = _uow.Mission.GetFirstOrDefault(mission => mission.MissionId == obj.MissionId);
                    GoalMission editGoalMission = _uow.GoalMisison.GetFirstOrDefault(goalMiss => goalMiss.MissionId == editThisMission.MissionId);
                    if(editThisMission != null)
                    {
                        editThisMission.Title = obj.Title;
                        editThisMission.ShortDescription = obj.ShortDescription;
                        editThisMission.Description = obj.Description;
                        editThisMission.CountryId = obj.CountryId;
                        editThisMission.CityId = obj.CityId;
                        editThisMission.OrganizationName = obj.OrganizationName;
                        editThisMission.OrganizationDetail = obj.OrganizationDetail;
                        editThisMission.StartDate = obj.StartDate;
                        editThisMission.EndDate = obj.EndDate;
                        editThisMission.MissionType = obj.MissionType;
                        editThisMission.TotalSeats = obj.TotalSeats;
                        editThisMission.MissionDeadline = obj.MissionDeadline;
                        editThisMission.MissionThemeId = obj.MissionThemeId;
                        editThisMission.Availability = obj.Availability;

                        _uow.Mission.Update(editThisMission);

                        if (editThisMission.MissionType == "Goal")
                        {
                            editGoalMission.GoalValue = obj.GoalValue;
                            editGoalMission.GoalObjectiveText = obj.GoalObjectiveText;

                            _uow.GoalMisison.Update(editGoalMission);
                            _uow.Save();
                        }

                        IEnumerable<MissionMedium> missionMediaList = _uow.MissionMedia.GetAccToFilter(media => media.MissionId == editThisMission.MissionId);
                        IEnumerable<MissionDocument> missionDocumentList = _uow.MissionDocuments.GetAccToFilter(media => media.MissionId == editThisMission.MissionId);
                        SaveMissionMediaAndDocs(editThisMission.MissionId, obj.VideoUrl, missionMediaList, missionDocumentList, MissionDocFiles, MissionImageFiles, preloadedmissdocs, preloadedmissimage);
                        var MissionSkillsId = _uow.MissionSkill.GetAccToFilter(missionSkill => missionSkill.MissionId == obj.MissionId).Select(missionSkill => missionSkill.SkillId);

                        AddMissionSkills(obj.MissionId, MissionSkillsId, finalMissSkillList);

                        _uow.Save();
                        return RedirectToAction("MissionDetails");

                    }
                }
            }

            MissionDetails missDetails = new()
            {
                CountryList = countryList,
                ThemeList = themeList,
                SkillList = skillList,
            };
            return View(obj);
        }

            //cascade city for mission edit get method
            public JsonResult EditMissCasCity(long countryId, long missionId)
        {
            Mission mission = new();
            long cityId = 0;
            if (missionId > 0)
            {
                mission = _uow.Mission.GetFirstOrDefault(mission => mission.MissionId == missionId);
                if (mission != null)
                {
                    cityId = mission.CityId;
                }
            }
            IEnumerable<City> cityList = _uow.City.GetAccToFilter(city => city.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }

        //delete mission post method
        [HttpPost]
        public IActionResult DeleteMission(long missId)
        {
            Mission mission = new();
            if (missId > 0)
            {
                mission = _uow.Mission.GetFirstOrDefault(mission => mission.MissionId == missId);
                if (mission != null)
                {
                    mission.DeletedAt = DateTime.Now;
                    mission.Status = 0;
                    _uow.Mission.Update(mission);
                    _uow.Save();
                    return RedirectToAction("MissionDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("MissionDetails");
        }


        //add edit mission skills
        public void AddMissionSkills(long missionId, IEnumerable<long> missionSkillsId, List<long> finalMissSkillList)
        {
            if (finalMissSkillList.Count == 0)
            {
                foreach (var mId in missionSkillsId)
                {
                    MissionSkill deleteSkill = _uow.MissionSkill.GetFirstOrDefault(missionSkill => missionSkill.MissionId == mId);
                    _uow.MissionSkill.Remove(deleteSkill);
                }
            }
            else
            {
                var skillToBeAdded = finalMissSkillList.Except(missionSkillsId);
                foreach (var skillId in skillToBeAdded)
                {
                    MissionSkill missSkills = new()
                    {
                        MissionId = missionId,
                        SkillId = skillId,
                    };
                    _uow.MissionSkill.Add(missSkills);
                }

                var skillToBeDeleted = missionSkillsId.Except(finalMissSkillList);
                foreach (var skillid in skillToBeDeleted)
                {
                    MissionSkill deleteSkill = _uow.MissionSkill.GetFirstOrDefault(missionSkill => missionSkill.SkillId == skillid);
                    _uow.MissionSkill.Remove(deleteSkill);
                }
            }
            _uow.Save();
        }
        //save mission media and documents post method
        [HttpPost]
        public void SaveMissionMediaAndDocs(long addedMissionId, string? videoUrl, IEnumerable<MissionMedium> missionMediaList, IEnumerable<MissionDocument> missionDocumentList, List<IFormFile> missionDocFiles, List<IFormFile> missionImageFiles, string[] preloadedmissdocs, string[] preloadedmissimage)
        {
            if (videoUrl != null)
            {
                foreach (var video in missionMediaList)
                {
                    if (video != null && video.MediaType == "video")
                    {
                        _uow.MissionMedia.Remove(video);
                    }
                }
                MissionMedium missMedForVid = new()
                {
                    MissionId = addedMissionId,
                    MediaName = "youtube viedo",
                    MediaType = "video",
                    MediaPath = videoUrl,
                };
                _uow.MissionMedia.Add(missMedForVid);
            }

            //for images
            foreach (var missionMedia in missionMediaList)
            {
                if (missionMedia.MediaType != "video")
                {
                    if (preloadedmissimage.Length < 1)
                    {
                        string missImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/", missionMedia.MediaPath);

                        if (System.IO.File.Exists(missImagePath))
                        {
                            System.IO.File.Delete(missImagePath);
                        }

                        _uow.MissionMedia.Remove(missionMedia);
                    }
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < preloadedmissimage.Length; i++)
                        {
                            string imgName = preloadedmissimage[i][14..];

                            if (imgName.Equals(missionMedia.MediaPath))
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/", missionMedia.MediaPath);

                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            _uow.MissionMedia.Remove(missionMedia);
                        }
                    }
                }
            }

            if (missionImageFiles?.Count > 0)
            {
                foreach (var image in missionImageFiles)
                {
                    string imgExt = Path.GetExtension(image.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        string imageNameForDb = image.FileName;
                        string imgSaveTo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/" + imageName);
                        using (FileStream stream = new(imgSaveTo, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        MissionMedium missionMed = new()
                        {
                            MissionId = addedMissionId,
                            MediaName = imageNameForDb,
                            MediaType = imgExt,
                            MediaPath = imageName,
                        };
                        _uow.MissionMedia.Add(missionMed);
                    }
                }
            }

            //for documents
            foreach (var missionDoc in missionDocumentList)
            {
                if (preloadedmissdocs.Length < 1)
                {
                    string missDocPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/", missionDoc.DocumentPath);

                    if (System.IO.File.Exists(missDocPath))
                    {
                        System.IO.File.Delete(missDocPath);
                    }

                    _uow.MissionDocuments.Remove(missionDoc);
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < preloadedmissdocs.Length; i++)
                    {
                        string docName = preloadedmissdocs[i][18..];

                        if (docName.Equals(missionDoc.DocumentPath))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        string docPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/", missionDoc.DocumentPath);

                        if (System.IO.File.Exists(docPath))
                        {
                            System.IO.File.Delete(docPath);
                        }

                        _uow.MissionDocuments.Remove(missionDoc);
                    }
                }
            }

            if (missionDocFiles?.Count > 0)
            {
                foreach (var docs in missionDocFiles)
                {
                    string docExt = Path.GetExtension(docs.FileName);
                    if (docExt == ".docx" || docExt == ".pdf" || docExt == ".xlsx")
                    {
                        string docName = docs.FileName;
                        string docSaveTo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/" + docName);
                        using (FileStream stream = new(docSaveTo, FileMode.Create))
                        {
                            docs.CopyTo(stream);
                        }
                        MissionDocument missionDoc = new()
                        {
                            MissionId = addedMissionId,
                            DocumentName = docName,
                            DocumentType = docExt,
                            DocumentPath = docName,
                        };
                        _uow.MissionDocuments.Add(missionDoc);
                    }
                }
            }
            _uow.Save();
        }

        //MissionTheme lists get method
        [HttpGet]
        public IActionResult MissionThemeDetails()
        {
            IEnumerable<MissionTheme> missionThemeLists = _uow.MissionTheme.GetAccToFilter(theme => theme.DeletedAt == null);
            MissionThemeDetails obj = new()
            {
                MissionThemeLists = missionThemeLists,
            };
            return View(obj);
        }

        //add mission theme get method
        [HttpGet]
        public IActionResult AddTheme()
        {
            return View();
        }

        //add theme post method
        [HttpPost]
        public IActionResult AddTheme(MissionThemeDetails obj)
        {
            if (obj != null)
            {
                MissionTheme missiontheme = new()
                {
                    Title = obj.Title,
                    Status = obj.Status,
                };

                _uow.MissionTheme.Add(missiontheme);
                _uow.Save();

                return RedirectToAction("MissionThemeDetails");
            }

            return View(obj);
        }

        //Edit mission theme get method
        [HttpGet]
        public IActionResult EditTheme(long themeId)
        {
            if (themeId > 0)
            {
                MissionTheme themedetails = _uow.MissionTheme.GetFirstOrDefault(theme => theme.MissionThemeId == themeId);
                if (themedetails != null)
                {
                    MissionThemeDetails obj = new()
                    {
                        MissionThemeId = themedetails.MissionThemeId,
                        Title = themedetails.Title,
                        Status = themedetails.Status,
                    };

                    return View(obj);
                }
            }

            return RedirectToAction("MissionThemeDetails");
        }

        //edit theme post method
        [HttpPost]
        public IActionResult EditTheme(MissionThemeDetails obj)
        {
            if (obj != null)
            {
                MissionTheme themedetails = _uow.MissionTheme.GetFirstOrDefault(theme => theme.MissionThemeId == obj.MissionThemeId);
                if (themedetails != null)
                {
                    themedetails.Title = obj.Title;
                    themedetails.Status = obj.Status;
                    themedetails.UpdatedAt = DateTime.Now;

                    _uow.MissionTheme.Update(themedetails);
                    _uow.Save();

                    return RedirectToAction("MissionThemeDetails");
                }
            }

            return View(obj);
        }

        //delete mission theme post method
        [HttpPost]
        public IActionResult DeleteTheme(long themeId)
        {
            MissionTheme theme = new();
            if (themeId > 0)
            {
                theme = _uow.MissionTheme.GetFirstOrDefault(theme => theme.MissionThemeId == themeId);
                if (theme != null)
                {
                    theme.DeletedAt = DateTime.Now;
                    theme.Status = 0;
                    _uow.MissionTheme.Update(theme);
                    _uow.Save();
                    return RedirectToAction("MissionThemeDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("MissionThemeDetails");
        }

        //MissionSkill Lists get method
        [HttpGet]
        public IActionResult SkillDetails()
        {
            IEnumerable<Skill> skillLists = _uow.Skill.GetAccToFilter(skill => skill.DeletedAt == null);
            SkillDetails obj = new()
            {
                SkillLists = skillLists,
            };
            return View(obj);
        }

        //add skills get method
        [HttpGet]
        public IActionResult AddSkill()
        {
            return View();
        }

        //add skills post method
        [HttpPost]
        public IActionResult AddSkill(SkillDetails obj)
        {
            if (obj != null)
            {
                Skill skill = new()
                {
                    SkillName = obj.SkillName,
                    Status = obj.Status,
                };

                _uow.Skill.Add(skill);
                _uow.Save();

                return RedirectToAction("SkillDetails");
            }

            return View(obj);
        }

        //Edit skills get method
        [HttpGet]
        public IActionResult EditSkill(long skillId)
        {
            if (skillId > 0)
            {
                Skill skilldetails = _uow.Skill.GetFirstOrDefault(skill => skill.SkillId == skillId);
                if (skilldetails != null)
                {
                    SkillDetails obj = new()
                    {
                        SkillId = skilldetails.SkillId,
                        SkillName = skilldetails.SkillName,
                        Status = skilldetails.Status,
                    };

                    return View(obj);
                }
            }

            return RedirectToAction("SkillDetails");
        }

        //edit skills post method
        [HttpPost]
        public IActionResult EditSkill(SkillDetails obj)
        {
            if (obj != null)
            {
                Skill skilldetails = _uow.Skill.GetFirstOrDefault(skill => skill.SkillId == obj.SkillId);
                if (skilldetails != null)
                {
                    skilldetails.SkillName = obj.SkillName;
                    skilldetails.Status = obj.Status;
                    skilldetails.UpdatedAt = DateTime.Now;

                    _uow.Skill.Update(skilldetails);
                    _uow.Save();

                    return RedirectToAction("SkillDetails");
                }
            }

            return View(obj);
        }

        //delete skill post method
        [HttpPost]
        public IActionResult DeleteSkill(long skillId)
        {
            Skill skill = new();
            if (skillId > 0)
            {
                skill = _uow.Skill.GetFirstOrDefault(skill => skill.SkillId == skillId);
                if (skill != null)
                {
                    skill.DeletedAt = DateTime.Now;
                    skill.Status = 0;
                    _uow.Skill.Update(skill);
                    _uow.Save();
                    return RedirectToAction("SkillDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("SkillDetails");
        }

        //MissionApplication Lists get method
        [HttpGet]
        public IActionResult MissionApplicationDetails()
        {
            IEnumerable<MissionApplication> missionAppLists = _uow.MissionApplication.GetAllMissAppList(missApp => missApp.ApprovalStatus == "PENDING");
            MissionApplicationDetails obj = new()
            {
                MissionAppLists = missionAppLists,
            };
            return View(obj);
        }

        //approve mission application
        public IActionResult AppMissApplication(long missAppId)
        {
            if (missAppId > 0)
            {
                MissionApplication thisMissApp = _uow.MissionApplication.GetFirstOrDefault(missApp => missApp.MissionApplicationId == missAppId);
                if (thisMissApp != null)
                {
                    thisMissApp.ApprovalStatus = "APPROVED";
                    _uow.MissionApplication.Update(thisMissApp);
                    _uow.Save();
                    TempData["success"] = "Mission approved successfully!";
                    return RedirectToAction("MissionApplicationDetails");
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("MissionApplicationDetails");
        }

        //decline mission application
        public IActionResult DecMissApplication(long missAppId)
        {
            if (missAppId > 0)
            {
                MissionApplication thisMissApp = _uow.MissionApplication.GetFirstOrDefault(missApp => missApp.MissionApplicationId == missAppId);
                if (thisMissApp != null)
                {
                    thisMissApp.ApprovalStatus = "DECLINE";
                    _uow.MissionApplication.Update(thisMissApp);
                    _uow.Save();
                    TempData["success"] = "Mission declined successfully!";
                    return RedirectToAction("MissionApplicationDetails");
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("MissionApplicationDetails");
        }

        //Story lists get method
        [HttpGet]
        public IActionResult StoryDetails()
        {
            IEnumerable<Story> storyLists = _uow.Story.GetAllStory(story => story.Status == "PENDING");
            StoryDetails obj = new()
            {
                StoryLists = storyLists,
            };
            return View(obj);
        }

        //view story get method
        [HttpGet]
        public IActionResult ViewStory(long storyId)
        {
            Story storyDetail = _uow.Story.GetStory(Story => Story.StoryId == storyId);
            if (storyDetail != null)
            {
                StoryDetails obj = new()
                {
                    StoryDetail = storyDetail,
                };

                return View(obj);
            }

            return RedirectToAction("StoryDetails");
        }

        //approve story application
        public IActionResult ApproveStory(long storyId)
        {
            if (storyId > 0)
            {
                Story thisStoryApp = _uow.Story.GetFirstOrDefault(story => story.StoryId == storyId);
                if (thisStoryApp != null)
                {
                    thisStoryApp.Status = "PUBLISHED";
                    _uow.Story.Update(thisStoryApp);
                    _uow.Save();
                    TempData["success"] = "Story approved successfully!";
                    return RedirectToAction("StoryDetails");
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("StoryDetails");
        }

        //decline story application
        public IActionResult DeclineStory(long storyId)
        {
            if (storyId > 0)
            {
                Story thisStoryApp = _uow.Story.GetFirstOrDefault(story => story.StoryId == storyId);
                if (thisStoryApp != null)
                {
                    thisStoryApp.Status = "DECLINED";
                    _uow.Story.Update(thisStoryApp);
                    _uow.Save();

                    TempData["success"] = "Story declined successfully!";
                    return RedirectToAction("StoryDetails");
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("StoryDetails");
        }

        //Banner Lists get method
        [HttpGet]
        public IActionResult BannerDetails()
        {
            IEnumerable<Banner> bannerLists = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null);
            BannerDetails obj = new()
            {
                BannerLists = bannerLists,
            };
            return View(obj);
        }

        //add banner get method
        [HttpGet]
        public IActionResult AddBanner()
        {
            return View();
        }

        //add banner post method
        [HttpPost]
        public IActionResult AddBanner(BannerDetails obj, IFormFile bannerImg)
        {
            if(obj != null)
            {
                Banner banner = new()
                {
                    Text = obj.Text,
                    SortOrder = obj.SortOrder,
                };
                if (bannerImg != null)
                {
                    string imgExt = Path.GetExtension(bannerImg.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string fileName = Path.Combine(bannerImg.FileName);
                        string imagePathForDb = Path.Combine("/BannerImages/" + fileName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imagePathForDb);
                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            bannerImg.CopyTo(stream);
                        }
                        banner.Image = imagePathForDb;
                    }
                }
                _uow.Banner.Add(banner);
                _uow.Save();

                return RedirectToAction("BannerDetails");
            }

            return View(obj);
        }

        //edit banner get method
        [HttpGet]
        public IActionResult EditBanner(long bannerId)
        {
            if (bannerId > 0)
            {
                Banner bannerDetails = _uow.Banner.GetFirstOrDefault(banner => banner.BannerId == bannerId);
                if(bannerDetails != null)
                {
                    BannerDetails obj = new()
                    {
                        BannerId = bannerDetails.BannerId,
                        Text = bannerDetails.Text,
                        SortOrder = bannerDetails.SortOrder,
                        Image = bannerDetails.Image,
                    };

                    return View(obj);
                }
            }

            return RedirectToAction("BannerDetails");
        }

        //edit banner post method
        [HttpPost]
        public IActionResult EditBanner(BannerDetails obj, IFormFile bannerImg)
        {
            if (obj?.BannerId != null)
            {
                Banner editBanner = _uow.Banner.GetFirstOrDefault(banner => banner.BannerId == obj.BannerId);

                editBanner.Text = obj.Text;
                editBanner.SortOrder = obj.SortOrder;
                editBanner.UpdatedAt = DateTime.Now;
                
                if (bannerImg != null)
                {
                    string imgExt = Path.GetExtension(bannerImg.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        if (editBanner.Image != null)
                        {
                            string fileName = Path.Combine(bannerImg.FileName);
                            string imagePathForDb = Path.Combine("/BannerImages/" + fileName);
                            string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imagePathForDb);
                            if (!imagePathForDb.Equals(editBanner.Image))
                            {
                                string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + editBanner.Image);
                                if (System.IO.File.Exists(alrExists))
                                {
                                    System.IO.File.Delete(alrExists);
                                }

                                using (FileStream stream = new(finalPath, FileMode.Create))
                                {
                                    bannerImg.CopyTo(stream);
                                }
                                editBanner.Image = imagePathForDb;
                            }
                        }
                    }
                }
                _uow.Banner.Update(editBanner);
                _uow.Save();

                return RedirectToAction("BannerDetails");
            }

            return View(obj);
        }

        //delete banner post method
        [HttpPost]
        public IActionResult DeleteBanner(long bannerId)
        {
            Banner banner = new();
            if (bannerId > 0)
            {
                banner = _uow.Banner.GetFirstOrDefault(banner => banner.BannerId == bannerId);
                if (banner != null)
                {
                    banner.DeletedAt = DateTime.Now;
                    _uow.Banner.Update(banner);
                    _uow.Save();
                    return RedirectToAction("BannerDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("BannerDetails");
        }
    }
}

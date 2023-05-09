using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace CIPlatformWeb.Controllers
{
    public class StoryController : CommonController
    {
        private readonly IUnitOfWork _uow;

        public StoryController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        //current user details
        public User GetThisUser()
        {
            ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
            string? email = identity?.FindFirst(ClaimTypes.Email)?.Value;
            User user = _uow.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //Story landing page
        public IActionResult StoriesPage()
        {
            StoryPageViewModel obj = new()
            {
                UserDetail = GetThisUser(),
            };
            return View(obj);
        }

        //method for loading card data
        public IActionResult StoryCardData(int pg = 1)
        {
            User user = GetThisUser();
            IEnumerable<StoryMedium> storyMedias = _uow.StoryMedia.GetAll();
            IEnumerable<Story> stories = _uow.Story.GetAccToFilter(story => story.Status == "PUBLISHED");
            IEnumerable<MissionTheme> missionThemeList = _uow.MissionTheme.GetAll();
            IEnumerable<Mission> missList = _uow.Mission.GetAll();
            IEnumerable<User> userList = _uow.User.GetAll();
            StoryPageViewModel obj = new()
            {
                MissionThemeList = missionThemeList,
                MissionList = missList,
                UsersList = userList,
                StoryList = stories.OrderByDescending(m => m.CreatedAt),
                UserDetail = user,
                StoryMediaList = storyMedias,
            };
            int resCount = obj.StoryList.Count();
            const int pageSize = 9;
            if (pg < 1)
                pg = 1;
            PaginationViewModel pager = new PaginationViewModel(resCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            obj.StoryList = obj.StoryList.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.storyPager = pager;

            return PartialView("_PartialViewForStoryCards", obj);
        }

        //shareyour story page
        public IActionResult ShareStory()
        {
            User user = GetThisUser();
            List<MissionApplication> draftMissAppList = new List<MissionApplication>();
            if (user != null)
            {
                IEnumerable<MissionApplication>? usersAppMission = _uow.MissionApplication.GetAccToFilter(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVED");
                foreach (var mission in usersAppMission)
                {
                    var story = _uow.Story.GetFirstOrDefault(m => m.MissionId == mission.MissionId && m.UserId == user.UserId);
                    if (story != null && (story.Status == "DRAFT" || story.Status == "DECLINED"))
                    {
                        draftMissAppList.Add(mission);
                    }
                    else if (story == null)
                    {
                        draftMissAppList.Add(mission);
                    }
                }
            }
            IEnumerable<Mission> missList = _uow.Mission.GetAll();
            IEnumerable<City> cityList = _uow.City.GetAll();
            StoryPageViewModel obj = new()
            {
                MissionList = missList,
                CityList = cityList,
                UserDetail = user,
                MissionApplicationList = draftMissAppList,
            };
            return View(obj);
        }

        [HttpPost]
        public void SaveMedia(long sId, String VideoUrls, IEnumerable<StoryMedium> storyMediaList, List<IFormFile> ImageFiles, string[] preloaded)
        {
            if (VideoUrls != null)
            {
                foreach (var video in storyMediaList)
                {
                    if (video != null && video.Type == "video")
                    {
                        _uow.StoryMedia.Remove(video);
                    }
                }
                StoryMedium strMedForVid = new()
                {
                    StoryId = sId,
                    Type = "video",
                    Path = VideoUrls,
                };
                _uow.StoryMedia.Add(strMedForVid);
            }

            //for images
            foreach (var storyMedia in storyMediaList)
            {
                if (storyMedia.Type != "video")
                {
                    if (preloaded.Length < 1)
                    {
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryMedia/", storyMedia.Path);

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                        _uow.StoryMedia.Remove(storyMedia);
                    }
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < preloaded.Length; i++)
                        {
                            string imgName = preloaded[i][12..];

                            if (imgName.Equals(storyMedia.Path))
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryMedia/", storyMedia.Path);

                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            _uow.StoryMedia.Remove(storyMedia);
                        }
                    }
                }
            }

            if (ImageFiles?.Count > 0)
            {
                foreach (var image in ImageFiles)
                {
                    string imgExt = Path.GetExtension(image.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        string imgSaveTo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryMedia/" + imageName);
                        using (FileStream stream = new(imgSaveTo, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        StoryMedium strMed = new()
                        {
                            StoryId = sId,
                            Type = imgExt,
                            Path = imageName,
                        };
                        _uow.StoryMedia.Add(strMed);
                    }
                }
            }

            _uow.Save();
        }

        //Save this Story
        [HttpPost]
        public IActionResult SaveThisStory(StoryPageViewModel obj, List<IFormFile> ImageFiles, string[] preloaded)
        {
            User user = GetThisUser();
            Story thisStory = _uow.Story.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == obj.MissionId);

            if (thisStory != null)
            {
                thisStory.Title = obj.Title;
                thisStory.Description = obj.Description;
                thisStory.MissionId = obj.MissionId;
                thisStory.UpdatedAt = DateTime.Now;
                thisStory.Status = "DRAFT";
                _uow.Story.Update(thisStory);
            }
            else
            {
                Story str = new()
                {
                    UserId = user.UserId,
                    MissionId = obj.MissionId,
                    Title = obj.Title,
                    Description = obj.Description,
                    CreatedAt = obj.DateCreated,
                    ViewCount = 0,
                };
                _uow.Story.Add(str);
                _uow.Save();
            }

            Story story = _uow.Story.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == obj.MissionId);
            IEnumerable<StoryMedium> storyMediaList = _uow.StoryMedia.GetAccToFilter(m => m.StoryId == story.StoryId);

            SaveMedia(story.StoryId, obj.VideoUrls, storyMediaList, ImageFiles, preloaded);

            return RedirectToAction("ShareStory");
        }

        //get drafted story data 
        public IActionResult GetDraftStoryData(int missId)
        {
            User user = GetThisUser();
            var storyDetail = new List<DraftedStoryDetails>();
            if (missId > 0)
            {
                storyDetail = _uow.Story.GetDraftedStoryDetails(missId, user.UserId);
            }

            if (storyDetail.Any())
            {
                return Json(storyDetail);
            }
            return Json(0);
        }

        //submit this story
        [HttpPost]
        public IActionResult SubmitThisStory(StoryPageViewModel obj, List<IFormFile> ImageFiles, string[] preloaded)
        {
            User? user = GetThisUser();
            Story? thisStory = new();
            if (obj != null)
            {
                thisStory = _uow.Story.GetFirstOrDefault(story => story.MissionId == obj.MissionId && story.UserId == user.UserId);
            }

            if (thisStory != null)
            {
                thisStory.Title = obj?.Title;
                thisStory.Description = obj?.Description;
                thisStory.CreatedAt = obj.DateCreated;
                thisStory.UpdatedAt = DateTime.Now;
                thisStory.Status = "PENDING";
                _uow.Story.Update(thisStory);
                _uow.Save();
            }

            Story story = _uow.Story.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == obj.MissionId);
            IEnumerable<StoryMedium> storyMediaList = _uow.StoryMedia.GetAccToFilter(m => m.StoryId == story.StoryId);

            SaveMedia(story.StoryId, obj.VideoUrls, storyMediaList, ImageFiles, preloaded);

            return RedirectToAction("ShareStory");
        }

        //Story detail page
        public IActionResult StoryDetail(int storyId, int views)
        {
            Story story = _uow.Story.GetStoryAndMedia(m => m.StoryId == storyId);
            if (story.ViewCount < views)
            {
                story.ViewCount = views;
                story.UpdatedAt = DateTime.Now;
                _uow.Story.Update(story);
                _uow.Save();
            }

            User user = GetThisUser();
            var userList = _uow.User.GetAll();
            var cityList = _uow.City.GetAll();
            var countryList = _uow.Country.GetAll();
            var storyDetail = _uow.Story.GetFirstOrDefault(m => m.StoryId == storyId);
            User userDetailOfThisStory = _uow.User.GetFirstOrDefault(m => m.UserId == storyDetail.UserId);
            StoryDetailViewModel obj = new()
            {
                UserDetail = user,
                UserDetailOfThisStory = userDetailOfThisStory,
                StoryDetail = storyDetail,
                UserList = userList,
                CityList = cityList,
                CountryList = countryList
            };
            return View(obj);
        }

        //recommend Story to user 
        [HttpPost]
        public void RecStoryToUser(int[]? userIds, long sId, int totalViews)
        {
            User thisUser = GetThisUser();
            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    User? user = _uow.User.GetFirstOrDefault(m => m.UserId == id);
                    StoryInvite obj = new()
                    {
                        StoryId = sId,
                        FromUserId = thisUser.UserId,
                        ToUserId = id,
                    };
                    _uow.StoryInvite.Add(obj);
                    _uow.Save();

                    var inviteLink = Url.Action("StoryDetail", "Story", new { storyId = sId, views = totalViews }, Request.Scheme);
                    var fromAddress = new MailAddress("jaypalsinhrana12345@gmail.com", "Jaypalsinh Rana");
                    var toAddress = new MailAddress(user.Email);
                    var subject = "Story Invite";
                    var body = $"Hi,<br /><br />My self {thisUser.FirstName + thisUser.LastName},<br /><br /> I invites you to read the Story for a Mission at CIPlatform.<br /><br />Click the following link to redirect to the page,<br /><br /><a href='{inviteLink}'>{inviteLink}</a>";
                    var msg = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential("jaypalsinhrana12345@gmail.com", "xsqqcdjjfsgffftg"),
                        EnableSsl = true,
                    };
                    try
                    {
                        SendMail(msg);
                    }
                    catch
                    {
                        TempData["error"] = "Opps! Something went wrong, try again later.";
                    }
                }
            }
        }
    }
}

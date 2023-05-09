using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using CIPlatformWeb.Hubs;
using CIPlatformWeb.Entities.ViewModels;

namespace CIPlatformWeb.Controllers
{
    public class CommonController : Controller
    {
        private readonly IUnitOfWork _uow;
        public CommonController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //get this user
        public User GetThisUser()
        {
            ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
            string? email = identity?.FindFirst(ClaimTypes.Email)?.Value;
            User? user = _uow.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //Add to Fav function 
        public void addToFav(int? missId)
        {
            User user = GetThisUser();
            if (missId != null && user?.UserId != null)
            {

                FavoriteMission favmission = _uow.FavoriteMission.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == missId);
                if (favmission == null)
                {
                    FavoriteMission fav = new()
                    {
                        UserId = user.UserId,
                        MissionId = (long)missId,
                    };
                    _uow.FavoriteMission.Add(fav);
                    _uow.Save();
                    TempData["info"] = "Mission added to favorite!";
                }
                else
                {
                    _uow.FavoriteMission.Remove(favmission);
                    _uow.Save();
                    TempData["info"] = "Mission removed from favorite!";
                }

            }
        }

        //recommend mission to user 
        public void recToUser(int[]? userIds, long missionId)
        {
            User thisUser = GetThisUser();
            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    User? user = _uow.User.GetFirstOrDefault(m => m.UserId == id);
                    MissionInvite obj = new()
                    {
                        MissionId = missionId,
                        ToUserId = user.UserId,
                        FromUserId = thisUser.UserId
                    };
                    _uow.MissionInvite.Add(obj);
                    _uow.Save();

                    string? inviteLink = Url.Action("VolunteeringMissionPage", "Home", new { id = missionId }, Request.Scheme);
                    MailAddress fromAddress = new("jaypalsinhrana12345@gmail.com", "Jaypalsinh Rana");
                    MailAddress toAddress = new(user.Email);
                    string subject = "Mission Invite";
                    string body = $"Hi,<br /><br /> you are invited by your friend to enroll to the mission at CIPlatform.<br /><br />Click the following link to get the details of mission,<br /><br /><a href='{inviteLink}'>{inviteLink}</a>";
                    MailMessage msg = new(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    SmtpClient smtpClient = new("smtp.gmail.com", 587)
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

        //send mail method
        public void SendMail(MailMessage msg)
        {
            SmtpClient smtpClient = new("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("jaypalsinhrana12345@gmail.com", "xsqqcdjjfsgffftg"),
                EnableSsl = true,
            };

            smtpClient.Send(msg);

        }

        //mark notification as readed
        public void MarkNotAsRead(long notId)
        {
            if(notId > 0)
            {
                NotifAlluser thisNotification = _uow.NotifToAll.GetFirstOrDefault(notif => notif.NotallId == notId);
                if(thisNotification != null)
                {
                    thisNotification.Isread = 1;
                    _uow.NotifToAll.Update(thisNotification);
                    _uow.Save();
                }
            }
        }


        //method for getting all the notification related data
        public IActionResult GetNotifData()
        {
            IEnumerable<NotiType> notificationTypeList = _uow.NotifType.GetAll();
            IEnumerable<NotifAlluser> notificationToAllList = _uow.NotifToAll.GetAll();
            NotificationData notifData = new()
            {
                NotificationTypeList = notificationTypeList,
                NotificationToAllList = notificationToAllList,
                NotificationCount = notificationToAllList.Count(notif => notif.Isread == 0),
            };
            return Json(notifData);
        }
    }
}

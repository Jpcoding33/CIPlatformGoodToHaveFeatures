using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace CIPlatformWeb.Controllers
{
    public class PolicyController : CommonController
    {
        private readonly IUnitOfWork _uow;
        public PolicyController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        public IActionResult PolicyAndCookies()
        {
            User? user = GetThisUser();
            if (user != null)
            {
                PolicyViewModel obj = new();
                obj.UserDetail = _uow.User.GetFirstOrDefault(u => u.UserId == user.UserId);
                return View(obj);
            }
            return View();
        }

        public IActionResult ContactUsMail(string mailSubject, string mailBody)
        {
            User user = GetThisUser();
            if (user != null)
            {
                ContactU contactDetail = new()
                {
                    UserId = user.UserId,
                    Subject = mailSubject,
                    Message = mailBody,
                };
                _uow.ContactUs.Add(contactDetail);
                _uow.Save();

                string body = $"HI ,<br /><br />{mailBody}<br /><br /> From {user.FirstName + " " + user.LastName}";
                MailMessage msg = new(user.Email, "jaypalsinhrana12345@gmail.com")
                {
                    Subject = mailSubject,
                    Body = body,
                    IsBodyHtml = true
                };

                try
                {
                    SendMail(msg);
                    return Json(1);
                }
                catch
                {
                    TempData["error"] = "Opps! Something went wrong, try again later.";
                }
            }
            return Json(0);
        }
    }
}

using CIPlatformWeb.Entities.Data;
using CIPlatformWeb.Entities.DataModels;
using CIPlatformWeb.Entities.ViewModels;
using CIPlatformWeb.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace CIPlatformWeb.Controllers
{
    public class AccountsController : CommonController
    {
        private readonly IUnitOfWork _uow;
        public AccountsController(IUnitOfWork uow) : base(uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            IEnumerable<Country> countryList = _uow.Country.GetAll();
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            RegistrationViewModel obj = new();
            obj.CountryList = countryList;
            obj.BannerList = bannerList;
            return View(obj);
        }

        public JsonResult RegCascadeCity(long countryId)
        {
            IEnumerable<City> cityList = _uow.City.GetAccToFilter(cities => cities.CountryId == countryId);
            return new JsonResult(cityList);
        }

        [HttpPost]
        public IActionResult Registration(RegistrationViewModel obj)
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            obj.BannerList = bannerList;
            User status = _uow.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower());
            if (status == null)
            {
                if (ModelState.IsValid)
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(obj.Password);
                    User user = new()
                    {
                        FirstName = obj.FirstName,
                        LastName = obj.LastName,
                        PhoneNumber = obj.PhoneNumber,
                        Email = obj.Email,
                        Password = passwordHash,
                        CountryId = obj.CountryId,
                        CityId = obj.CityId,
                    };
                    _uow.User.Add(user);
                    _uow.Save();
                    TempData["success"] = "Registration done successfully!";
                    return RedirectToAction("Login", "Accounts");
                }
            }
            TempData["error"] = "Email Already Exists!";
            return View(obj);
        }

        //login method
        [HttpGet]
        public IActionResult Login()
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            LoginViewModel obj = new()
            {
                BannerList = bannerList,
            };
            return View(obj);
            //return View();
        }

        //login post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            LoginViewModel loginModel = new()
            {
                BannerList = bannerList,
            };
            if (ModelState.IsValid)
            {
                User user = _uow.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower() && m.Status == 1 && m.DeletedAt == null);
                if (user != null && BCrypt.Net.BCrypt.Verify(obj.Password, user.Password))
                {
                    ClaimsIdentity identity = new(new[] {new Claim(ClaimTypes.Email, user.Email)},
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.LastName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
                    ClaimsPrincipal principal = new(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false,
                    };
                    await HttpContext.SignInAsync(principal, authProperties);
                    HttpContext.Session.SetString("emailId", user.Email);
                    TempData["success"] = "Logged In successfully!";

                    //user.Status = 1;
                    //_uow.SaveChanges();
                    if(user.Role == "USER")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("UserDetail", "Admin");
                    }
                }

                TempData["error"] = "Invalid Email Or Password!";
                return View(loginModel);
            }

            return View(loginModel);
        }

        //forgot password get method
        [HttpGet]
        public IActionResult Forgot_Password()
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            FPViewModel obj = new()
            {
                BannerList = bannerList
            };
            return View(obj);
        }

        //forgot password post method
        [HttpPost]
        public IActionResult Forgot_Password(FPViewModel obj)
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            FPViewModel fpmodel = new()
            {
                BannerList = bannerList
            };
            User IsValidEmail = _uow.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower());
            if (IsValidEmail == null)
            {
                TempData["error"] = "Email doesn't exists!";
                return View(fpmodel);
            }
            var token = Guid.NewGuid().ToString();

            PasswordReset passReset = new PasswordReset
            {
                Email = obj.Email,
                Token = token,
            };
            _uow.PasswordReset.Add(passReset);
            _uow.Save();

            string? resetLink = Url.Action("Reset_Password", "Accounts", new { email = obj.Email, token }, Request.Scheme);

            MailAddress fromAddress = new("jaypalsinhrana12345@gmail.com", "Jaypalsinh Rana");
            MailAddress toAddress = new(obj.Email);
            string subject = "Reset Password";
            string body = $"Hi,<br /><br />Please chick on the following link to reset your password,<br /><br /><a href='{resetLink}'>{resetLink}</a>";
            MailMessage msg = new(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            try
            {
                SendMail(msg);
                TempData["success"] = "Reset link sent successfully!";
                return RedirectToAction("Login", "Accounts");
            }
            catch
            {
                TempData["error"] = "Opps! Something went wrong, try again later.";
                return RedirectToAction("Forgot_Password", "Accounts");
            }
        }

        [HttpGet]
        public IActionResult Reset_Password(string email, string token)
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            RPViewModel obj = new()
            {
                BannerList = bannerList,
            };
            PasswordReset status = _uow.PasswordReset.GetFirstOrDefault(m => m.Email.ToLower() == email.ToLower() && m.Token == token);
            if (status == null)
            {
                return RedirectToAction("Forgot_Password", "Accounts");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Reset_Password(RPViewModel obj)
        {
            IEnumerable<Banner> bannerList = _uow.Banner.GetAccToFilter(banner => banner.DeletedAt == null).OrderBy(banner => banner.SortOrder);
            RPViewModel rpmodel = new()
            {
                BannerList = bannerList,
            };
            if (ModelState.IsValid)
            {
                User user = _uow.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.ToLower());
                if(user == null)
                {
                    TempData["error"] = "EmailID not found";
                    return View();
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(obj.Password);
                user.Password = passwordHash;
                user.UpdatedAt = DateTime.Now;
                _uow.Save();
                TempData["success"] = "Password Reset successfully!";
                return RedirectToAction("Login", "Accounts");

            }
            TempData["error"] = "Something went wrong!";
            return View(rpmodel);
            
        }

        public IActionResult PageNotFound()
        {
            return View();
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using ProjectZ.Web.Helpers;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.Controllers
{
    public class UserController : RavenController
    {


        public ActionResult Index()
        {
            return View();
        }

        [GET("User/Details/{userName}")]
        public ActionResult Details(string userName)
        {

            var user = RavenSession.Query<User>().FirstOrDefault(x => x.Slug == userName);

            if (user == null)
                throw new HttpException(404, "Post not found");

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = RavenSession.Query<User>().FirstOrDefault(x => x.UserName == username || x.Email == username);

            if (user == null)
                return View();


            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return View();


            LoginUser(user.Id);


            return View();
        }

        public JsonResult Search(string q)
        {
            return Json(RavenSession.Query<User>().Where(x => x.UserName.StartsWith(q) || x.Email.StartsWith(q)).ToList().Select(x => new TeamMember(x, Role.Developer, false)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Manage()
        {
            if (CurrentUser == null)
                return Redirect("/user/login");

            return View();
        }

        [HttpPost]
        public ActionResult Manage(User user)
        {
            if (CurrentUser == null)
                return Redirect("/user/login");

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public void LoginUser(string userId)
        {
            FormsAuthentication.SetAuthCookie(userId, true);
        }

        [GET("User/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {

            user.Created = DateTime.Now;
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Slug = user.UserName.GenerateSlug();

            if (string.IsNullOrEmpty(user.GravatarEmail))
                user.GravatarEmail = user.Email;

            RavenSession.Store(user);

            return Redirect("/user/" + user.Slug);

            return View();
        }

        public JsonResult CheckUsernameAvailability(string username)
        {
            return Json(new { success = RavenSession.Query<User>().FirstOrDefault(x => x.Slug == username.GenerateSlug()) == null, message = "This username is taken" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEmailAvailability(string email)
        {
            return Json(new { success = RavenSession.Query<User>().FirstOrDefault(x => x.Email.ToLower() == email.ToLower()) == null, message = "This email is in use" }, JsonRequestBehavior.AllowGet);
        }


    }


}

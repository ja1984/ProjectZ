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
using ProjectZ.Web.Models.ViewModels;

namespace ProjectZ.Web.Controllers
{

    public class UserController : RavenController
    {
        [GET("Users")]
        public ActionResult Index()
        {
            var users = RavenSession.Query<User>().ToList();

            return View(users);
        }

        [GET("User/{userName}/Details")]
        public ActionResult Details(string userName)
        {
            var user = RavenSession.Query<User>().FirstOrDefault(x => x.Slug == userName);

            if (user == null)
                throw new HttpException(404, "User not found");

            return View(new UserViewModel { User = user });
        }


        [GET("User/{userName}/Edit")]
        public ActionResult Edit(string userName)
        {
            var user = RavenSession.Query<User>().FirstOrDefault(x => x.Slug == userName);

            if (user == null)
                throw new HttpException(404, "User not found");

            if (CurrentUser.Id != user.Id)
                throw new HttpException(403, "You can not edit this user");


            return View(new UserViewModel { User = user });
        }

        [POST("User/Edit/{userName}")]
        public ActionResult Edit(User user)
        {
            var oldUser = RavenSession.Load<User>(CurrentUser.Id);

            oldUser.GravatarEmail = user.GravatarEmail;
            oldUser.Email = user.Email;
            oldUser.FirstName = user.FirstName;
            oldUser.LastName = user.LastName;
            oldUser.Description = user.Description;
            oldUser.DisplayEmail = user.DisplayEmail;
            oldUser.GitHub = user.GitHub;

            RavenSession.SaveChanges();


            return RedirectToAction("edit", "user", new { username = CurrentUser.Slug });
        }


        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginModel)
        {
            var user = RavenSession.Query<User>().FirstOrDefault(x => x.UserName == loginModel.Username || x.Email == loginModel.Username);

            if (user == null)
            {
                loginModel.HasError = true;
                loginModel.Error = "Wrong username/email";
                return View(loginModel);
            }



            if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                loginModel.HasError = true;
                loginModel.Error = "Wrong password";
                return View(loginModel);
            }


            LoginUser(user.Id);


            return RedirectToAction("Details", "User", new { userName = user.Slug });
        }

        public JsonResult Search(string q)
        {
            return Json(RavenSession.Query<User>().Where(x => x.UserName.StartsWith(q) || x.Email.StartsWith(q)).ToList().Select(x => new TeamMember(x, Role.Developer, true).SmallInfo()), JsonRequestBehavior.AllowGet);
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

            return Redirect("/user/" + user.Slug + "/edit");

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

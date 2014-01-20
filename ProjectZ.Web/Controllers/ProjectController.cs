using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using ProjectZ.Web.Helpers;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.Controllers
{
    public class ProjectController : RavenController
    {
        //
        // GET: /Project/

        public ActionResult Index()
        {
            return View();
        }

        [GET("Details/{projectName}")]
        public ActionResult Details(string projectName)
        {
            return View();
        }

        [GET("Manage/{projectName}")]
        public ActionResult Manage(string projectName)
        {

            if (CurrentUser == null)
                return Redirect("/user/login");


            var project = RavenSession.Query<Project>().FirstOrDefault(x => x.UserId == CurrentUser.Id || x.Admins.Contains(CurrentUser.Id));
            if (project == null)
                throw new HttpException(403, "You are not admin of this project");


            return View();
        }

        public ActionResult Create()
        {

            if (CurrentUser == null)
                return Redirect("/user/login");


            return View();
        }

        [HttpPost]
        public ActionResult Create(Project project)
        {

            project.Created = DateTime.Now;
            project.DisplayName = project.Name.GenerateSlug();
            project.UserId = CurrentUser.Id;

            RavenSession.Store(project);

            return Redirect("/project/details/" + project.DisplayName);

            return View();
        }

    }
}

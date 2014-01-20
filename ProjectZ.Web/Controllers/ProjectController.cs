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

        public ActionResult Details()
        {
            return View();
        }

        public ActionResult Manage()
        {

            if (CurrentUser == null)
                return Redirect("/user/login");
            

            var project = RavenSession.Query<Project>().FirstOrDefault();


            if (project == null)
                throw new HttpException(404, "No project whit this name");

            if (project.Admins.FirstOrDefault(x => x.Id == CurrentUser.Id) == null)
                throw new HttpException(403, "You are not admin of this project");


            return View(project);
        }

        public ActionResult Create()
        {

            if (CurrentUser == null)
                return Redirect("/user/login");


            return View();
        }

        [HttpPost]
        public ActionResult Create(Project project, Role role)
        {

            project.Created = DateTime.Now;
            project.DisplayName = project.Name.GenerateSlug();
            project.Admins.Add(new TeamMember(CurrentUser, role, true));
            RavenSession.Store(project);

            return Redirect("/project/details/" + project.DisplayName);

            return View();
        }

    }
}

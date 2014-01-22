using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using ProjectZ.Web.Helpers;
using ProjectZ.Web.Models;
using ProjectZ.Web.ViewModels;

namespace ProjectZ.Web.Controllers
{
    public class ProjectController : RavenController
    {
        //
        // GET: /Project/

        public ActionResult Index()
        {
            var projects = RavenSession.Query<Project>().ToList();
            return View(projects);
        }

        public ActionResult Details()
        {
            var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);

            if (project == null)
                return RedirectToAction("Index");

            var pageAdmins = project.Admins.Where(x => x.IsPageAdmin).ToList();

            return View(new ProjectViewModel { Project = project, IsPageAdmin = CurrentUser != null && pageAdmins.Select(x => x.Id).Contains(CurrentUser.Id) });
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

        public JsonResult AddTeamMember(TeamMember teamMember, string projectId)
        {

            var project = RavenSession.Load<Project>(projectId);

            if(project == null)
                return Json("Error!");


            project.Admins.Add(teamMember);

            RavenSession.SaveChanges();


            return Json("");
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
            project.Slug = project.Name.GenerateSlug();
            project.Admins.Add(new TeamMember(CurrentUser, role, true));
            RavenSession.Store(project);

            return Redirect("/project/details/" + project.Slug);

            return View();
        }

    }
}

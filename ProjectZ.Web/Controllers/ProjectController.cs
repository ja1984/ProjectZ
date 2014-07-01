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

        [GET("Projects")]
        public ActionResult Index()
        {
            var projects = RavenSession.Query<Project>().ToList();
            return View(projects);
        }

        [GET("Projects/{id}/{projectName}/Details")]
        public ActionResult Details(int id, string projectName)
        {
            var project = RavenSession.Load<Project>(id);

            if (project == null)
                return RedirectToAction("Index");

            var issues = RavenSession.Query<Issue>().Count(x => x.ProjectId == project.Id);
            var releases = RavenSession.Query<Release>().Count(x => x.ProjectId == project.Id);

            var pageAdmins = project.Admins.Where(x => x.IsPageAdmin).ToList();

            return View(new ProjectViewModel { Project = project, IsPageAdmin = CurrentUser != null && pageAdmins.Select(x => x.UserId).Contains(CurrentUser.Id), NumberOfIssues = issues, NumberOfReleases = releases });
        }

        [GET("Projects/{id}/{projectName}/Issues")]
        public ActionResult Issues(int id, string projectName)
        {
            var project = RavenSession.Load<Project>(id);
            var issues = RavenSession.Query<Issue>().Where(x => x.ProjectId == project.Id).ToList();
            var releases = RavenSession.Query<Release>().Count(x => x.ProjectId == project.Id);

            return View(new IssueViewModel { Project = project, Issues = issues, UserId = CurrentUser == null ? string.Empty : CurrentUser.Id, NumberOfReleases = releases });
        }

        [GET("Projects/{id}/{projectName}/Releases")]
        public ActionResult Releases(int id, string projectName)
        {
            var project = RavenSession.Load<Project>(id);
            var releases = RavenSession.Query<Release>().Where(x => x.ProjectId == project.Id).OrderByDescending(x => x.Created).ToList();
            var issues = RavenSession.Query<Issue>().Count(x => x.ProjectId == project.Id);
            return View(new ReleaseViewModel { NumberOfIssues = issues, Project = project, Releases = releases });
        }

        [GET("Projects/{id}/{projectName}/Polls")]
        public ActionResult Polls(int id, string projectName)
        {
            var project = RavenSession.Load<Project>(id);
            var releases = RavenSession.Query<Release>().Count(x => x.ProjectId == project.Id);
            var issues = RavenSession.Query<Issue>().Count(x => x.ProjectId == project.Id);
            return View(new PollViewModel { NumberOfIssues = issues, Project = project, NumberOfReleases = releases });
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

            if (project == null)
                return Json(new { success = false, message = "Couldn´t find project" });

            if (CurrentUser == null)
                return Json(new { success = false, message = "You are not admin of this project" });

            if (!project.Admins.Select(x => x.Id).Contains(CurrentUser.Id))
                return Json(new { success = false, message = "You are not admin of this project" });


            project.Admins.Add(teamMember);
            RavenSession.SaveChanges();


            return Json(new { success = true, message = "User added" });

        }

        public ActionResult Create()
        {

            if (CurrentUser == null)
                return Redirect("/user/login");


            return View();
        }

        [HttpPost]
        public JsonResult Create(Project project, Role role)
        {

            project.Created = DateTime.Now;
            project.Slug = project.Name.GenerateSlug();
            project.Admins.Add(new TeamMember(CurrentUser, role, true));
            RavenSession.Store(project);
            var url = string.Format("/{0}/{1}/details/", project.Id, project.Slug);

            return Json(new { Success = true, Url = url });
        }

    }
}

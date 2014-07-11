using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using ImageResizer;
using ProjectZ.Web.Helpers;
using ProjectZ.Web.Models;
using ProjectZ.Web.Models.ViewModels;
using ProjectZ.Web.ViewModels;
using Action = ProjectZ.Web.Models.Action;

namespace ProjectZ.Web.Controllers
{
    public class ProjectController : RavenController
    {
        //
        // GET: /Project/

        [GET("Projects")]
        public ActionResult List()
        {
            var projects = RavenSession.Query<Project>().ToList();
            return View(projects);
        }

        [GET("Projects/{id}/{projectName}/Edit")]
        public ActionResult Edit(int id, string projectName)
        {
            var project = RavenSession.Load<Project>(id);

            if (project == null)
                throw new HttpException(404, "No project");

            if (CurrentUser == null)
                throw new HttpException(403, "You are not logged in");

            if (!project.Admins.Select(x => x.UserId).Contains(CurrentUser.Id))
                throw new HttpException(403, "You are not logged in");


            return View(project);
        }

        [GET("Projects/{id}/{projectName}/Issues/{issueId}")]
        public ActionResult Issue(int id, string projectName, int issueId)
        {
            var project = RavenSession.Load<Project>(id);
            var issue = project.Issues.FirstOrDefault(x => x.Id == issueId);

            return View(new IssueViewModel() { Issue = issue, ProjectId = project.Id });
        }

        [GET("Projects/{id}/{projectName}/{startpage?}")]
        public ActionResult Index(int id, string projectName, string startPage)
        {
            var project = RavenSession.Load<Project>(id);

            var test = HttpContext.Request.Url;

            if (project == null)
                return RedirectToAction("Index");

            var releases = RavenSession.Query<Release>().Where(x => x.ProjectId == project.Id).ToList();
            var pageAdmins = project.Admins.Where(x => x.IsPageAdmin).ToList();

            if (string.IsNullOrEmpty(startPage))
                startPage = "overview";

            return View(new ProjectViewModel { Project = project, IsPageAdmin = CurrentUser != null && pageAdmins.Select(x => x.UserId).Contains(CurrentUser.Id), Releases = releases, Followers = project.Followers.Count(), Following = CurrentUser == null || project.Followers.Contains(CurrentUser.Id), StartPage = startPage.ToLower() });
        }


        [HttpPost]
        public JsonResult CreatePoll(string projectId, Poll poll)
        {

            var project = RavenSession.Load<Project>(projectId);

            var eventAction = new EventAction()
            {
                Action = Action.Poll,
                Created = DateTime.Now,
                ProjectId = projectId,
                Title = poll.Title,
                ProjectName = project.Name,
                Url = string.Format("/{0}/{1}/polls", projectId, project.Slug),
                User = new EventActionUser(CurrentUser),
                Reference = new EventActionReference
                                {
                                    Id = project.Id,
                                    Name = project.Name
                                }
            };
            RavenSession.Store(eventAction);

            project.Polls.Add(poll);
            RavenSession.SaveChanges();

            return Json("");
        }




        //[GET("Projects/{id}/{projectName}/Releases")]
        //public ActionResult Releases(int id, string projectName)
        //{
        //    var project = RavenSession.Load<Project>(id);
        //    var releases = RavenSession.Query<Release>().Where(x => x.ProjectId == project.Id).OrderByDescending(x => x.Created).ToList();
        //    return View(new ReleaseViewModel { NumberOfIssues = project.Issues.Count(), Project = project, Releases = releases });
        //}

        //[GET("Projects/{id}/{projectName}/Polls")]
        //public ActionResult Polls(int id, string projectName)
        //{
        //    var project = RavenSession.Load<Project>(id);
        //    var releases = RavenSession.Query<Release>().Count(x => x.ProjectId == project.Id);
        //    return View(new PollViewModel { NumberOfIssues = project.Issues.Count(), Project = project, NumberOfReleases = releases });
        //}

        [HttpPost]
        public JsonResult Follow(string projectId)
        {

            if (CurrentUser == null)
                return Json(false);

            var project = RavenSession.Load<Project>(projectId);
            var currentUser = RavenSession.Load<User>(CurrentUser.Id);

            if (project == null || currentUser == null)
                return Json(false);

            if (project.Followers.Contains(currentUser.Id))
                return Json(false);

            project.Followers.Add(CurrentUser.Id);
            currentUser.Follows.Add(new Follow(project));
            RavenSession.SaveChanges();


            return Json(true);
        }

        [HttpPost]
        public JsonResult UnFollow(string projectId)
        {

            if (CurrentUser == null)
                return Json(false);

            var project = RavenSession.Load<Project>(projectId);
            var currentUser = RavenSession.Load<User>(CurrentUser.Id);

            if (project == null || currentUser == null)
                return Json(false);

            project.Followers.Remove(CurrentUser.Id);
            var follow = currentUser.Follows.FirstOrDefault(x => x.Id == projectId);
            if (follow == null)
                return Json(false);

            currentUser.Follows.Remove(follow);
            RavenSession.SaveChanges();

            return Json(true);
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

        public JsonResult AddTeamMember(string projectId, string userId, bool isAdmin, Role role)
        {

            var project = RavenSession.Load<Project>(projectId);

            if (project == null)
                return Json(new { success = false, message = "Couldn´t find project" });

            var user = RavenSession.Load<User>(userId);

            if (user == null)
                return Json(new { success = false, message = "Couldn´t find user" });

            if (CurrentUser == null)
                return Json(new { success = false, message = "You are not admin of this project" });

            if (!project.Admins.Where(x => x.IsPageAdmin).Select(x => x.UserId).Contains(CurrentUser.Id))
                return Json(new { success = false, message = "You are not admin of this project" });


            project.Admins.Add(new TeamMember(user, role, isAdmin));

            var eventAction = new EventAction()
            {
                Action = Action.TeamMember,
                Created = DateTime.Now,
                ProjectId = projectId,
                ProjectName = project.Name,
                Title = user.UserName,
                Url = string.Format("/user/{0}", user.Slug),
                User = new EventActionUser(CurrentUser),
                Reference = new EventActionReference
                {
                    Id = project.Id,
                    Name = project.Name
                }
            };

            RavenSession.Store(eventAction);

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
            var user = RavenSession.Load<User>(CurrentUser.Id);

            if (user.Projects == null)
                user.Projects = new List<UserProject>();

            user.Projects.Add(new UserProject(project));
            RavenSession.SaveChanges();
            var url = string.Format("/{0}/{1}", project.Id, project.Slug);

            return Json(new { Success = true, Url = url });
        }

        [HttpPost]
        public JsonResult SaveLogo(string projectId, string url)
        {
            var project = RavenSession.Load<Project>(projectId);
            project.Logo = url;
            RavenSession.SaveChanges();

            return Json("");
        }

        [HttpPost]
        public JsonResult RemoveLogo(string projectId, string url)
        {
            var project = RavenSession.Load<Project>(projectId);
            project.Logo = string.Empty;
            RavenSession.SaveChanges();

            var id = StringHelper.GetProjectId(projectId);



            var uploadUrl = Server.MapPath("/Uploads") + "\\";

            var logoUrl = uploadUrl + "logo_";
            var logo = ImageUtils.GetImage(id, logoUrl + id);
            if (!string.IsNullOrEmpty(logo))
                System.IO.File.Delete(logoUrl + logo);

            var iconUrl = uploadUrl + "icon_";
            var icon = ImageUtils.GetImage(id, iconUrl + id);
            if (!string.IsNullOrEmpty(icon))
                System.IO.File.Delete(iconUrl + icon);

            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Upload(HttpPostedFileBase uploadedFile, string projectId)
        {
            // Validate the uploaded file
            if (uploadedFile == null || uploadedFile.ContentType != "image/jpeg")
            {
                // Return bad request error code
                return Json(new
                {
                    statusCode = 400,
                    status = "Bad Request! Upload Failed",
                    file = string.Empty
                }, "text/html");
            }

            // Save the file to the server



            var path = Server.MapPath("/Uploads") + "\\";
            var completePath = path + uploadedFile.FileName;
            var fileExtension = Path.GetExtension(completePath);
            var newFileName = StringHelper.GetProjectId(projectId) + fileExtension;
            completePath = path + newFileName;

            uploadedFile.SaveAs(completePath);

            ResizeImage(completePath, path, "logo_" + newFileName);
            ResizeImage(completePath, path, "icon_" + newFileName, 24, 24, completePath);

            // Return success code
            return Json(new
            {
                statusCode = 200,
                status = "Image uploaded.",
                file = "logo_" + newFileName,
            }, "text/html");
        }

        private void ResizeImage(String image, string location, string fileName, int height = 104, int width = 104, string original = null)
        {
            var srcImage = Image.FromFile(image);
            var imageJob = new ImageJob(srcImage, string.Format("{0}{1}", location, fileName), new ResizeSettings(width, height, FitMode.Max, null));
            try
            {
                imageJob.Build();
                srcImage.Dispose();

                if (!string.IsNullOrEmpty(original))
                    System.IO.File.Delete(original);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}

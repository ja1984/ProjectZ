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

        [GET("Projects/{id}/{projectName}/Edit/{startpage?}")]
        public ActionResult Edit(int id, string projectName, string startPage)
        {
            var project = RavenSession.Load<Project>(id);
            var releases = RavenSession.Query<Release>().Where(x => x.ProjectId == project.Id).ToList();
            if (project == null)
                throw new HttpException(404, "No project");

            if (CurrentUser == null)
                throw new HttpException(403, "You are not logged in");

            if (!project.Admins.Select(x => x.UserId).Contains(CurrentUser.Id))
                throw new HttpException(403, "You are not logged in");

            if (string.IsNullOrEmpty(startPage))
                startPage = "description";

            return View(new EditProjectViewModel { Project = project, StartPage = startPage, Releases = releases });
        }

        [GET("Projects/{id}/{projectName}/Issues/{issueId}")]
        public ActionResult Issue(int id, string projectName, int issueId)
        {
            var project = RavenSession.Load<Project>(id);
            var issue = project.Issues.FirstOrDefault(x => x.Id == issueId);

            return View(new IssueViewModel() { Issue = issue, ProjectId = project.Id });
        }

        [GET("Projects/{id}/{projectName}/Releases/{releaseId}")]
        public ActionResult Release(int id, string projectName, int releaseId)
        {
            var project = RavenSession.Load<Project>(id);
            var release = RavenSession.Load<Release>(string.Format("releases/{0}", releaseId));
            var markdownText = new MarkdownSharp.Markdown().Transform(release.Description);

            return View(new ReleaseViewModel { Description = markdownText, Title = release.Title, SolvedIssues = release.Issues.ToList(), Version = release.Version, Project = project });
        }

        [GET("Projects/{id}/{projectName}/{startpage?}")]
        public ActionResult Index(int id, string projectName, string startPage)
        {
            var project = RavenSession.Load<Project>(id);

            if (project == null)
                return RedirectToAction("Index");

            if (project.IsPrivate && !HttpContext.User.Identity.IsAuthenticated)
                throw new HttpException(403, "You must be logged in to view this project!");


            var releases = RavenSession.Query<Release>().Where(x => x.ProjectId == project.Id).ToList();
            var pageAdmins = project.Admins.Where(x => x.IsPageAdmin).ToList();
            var events = RavenSession.Query<EventAction>().Where(x => x.ProjectId == string.Format("projects/{0}", id)).ToList();

            var projectDescription = new MarkdownSharp.Markdown().Transform(project.Description);

            if (string.IsNullOrEmpty(startPage))
                startPage = "overview";

            return View(new ProjectViewModel { Project = project, Events = events, IsPageAdmin = CurrentUser != null && pageAdmins.Select(x => x.UserId).Contains(CurrentUser.Id), Releases = releases, Followers = project.Followers.Count(), Following = CurrentUser == null || project.Followers.Contains(CurrentUser.Id), StartPage = startPage.ToLower(), ProjectDescription = projectDescription });
        }

        //[POST("Project/CreateRelease")]
        //public ActionResult SaveRelease(CreateReleaseModel release)
        //{
        //    var projet = RavenSession.Load<Project>(release.ProjectId);
        //    var issues = new List<Issue>();
        //    if (release.SolvedIssues != null)
        //    {
        //        issues.AddRange(projet.Issues.Where(x => release.SolvedIssues.Contains(x.Id)).ToList());

        //        foreach (var issue in issues)
        //        {
        //            issue.Solved = true;
        //        }
        //    }

        //    var newRelease = new Release
        //                         {
        //                             Created = DateTime.Now,
        //                             Description = release.Description,
        //                             Issues = issues,
        //                             ProjectId = release.ProjectId,
        //                             Title = release.Title,
        //                             Version = release.Version
        //                         };

        //    RavenSession.Store(newRelease);
        //    RavenSession.SaveChanges();

        //    return View();
        //}

        [HttpPost]
        public JsonResult SaveDescription(string projectId, string text)
        {
            var project = RavenSession.Load<Project>(projectId);
            project.Description = text;

            RavenSession.SaveChanges();

            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult CreateQuestion(string projectId, Question question)
        {
            var project = RavenSession.Load<Project>(projectId);
            question.Id = project.Questions.Count() + 1;

            project.Questions.Add(question);

            RavenSession.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult CreatePoll(string projectId, Poll poll)
        {

            var project = RavenSession.Load<Project>(projectId);

            poll.Id = project.Polls.Count() + 1;

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

            return Json(new { success = true });
        }

        public JsonResult DeletePoll(string projectId, int pollId)
        {
            var project = RavenSession.Load<Project>(projectId);
            var poll = project.Polls.FirstOrDefault(x => x.Id == pollId);
            project.Polls.Remove(poll);
            RavenSession.SaveChanges();

            return Json(new { success = true });
        }

        public JsonResult DeleteTeamMember(string projectId, string userId)
        {
            var project = RavenSession.Load<Project>(projectId);
            var user = project.Admins.FirstOrDefault(x => x.UserId == userId);

            var _user = RavenSession.Load<User>(userId);

            var _project = _user.Projects.FirstOrDefault(x => x.Id == project.Id);
            _user.Projects.Remove(_project);


            if (user.IsCreator || project.Admins.Count() == 1)
                return Json(new { success = false });

            project.Admins.Remove(user);
            RavenSession.SaveChanges();

            return Json(new { success = true });
        }

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

        public JsonResult AddTeamMember(string projectId, string userId, bool isAdmin, Role role)
        {

            var project = RavenSession.Load<Project>(projectId);

            if (project == null)
                return Json(new { success = false, message = "Couldn´t find project" });

            if (project.Admins.Select(x => x.UserId).Contains(userId))
                return Json(new { success = false, message = "This user is already part of team" });

            var user = RavenSession.Load<User>(userId);

            if (user == null)
                return Json(new { success = false, message = "Couldn´t find user" });

            if (CurrentUser == null)
                return Json(new { success = false, message = "You are not admin of this project" });

            if (!project.Admins.Where(x => x.IsPageAdmin).Select(x => x.UserId).Contains(CurrentUser.Id))
                return Json(new { success = false, message = "You are not admin of this project" });

            user.Projects.Add(new UserProject(project));
            var teamMember = new TeamMember(user, role, isAdmin);
            project.Admins.Add(teamMember);

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


            return Json(new { success = true, message = "User added", User = teamMember });

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
            project.Admins.Add(new TeamMember(CurrentUser, role, true, true));
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
        public JsonResult SaveLogo(string projectId, string logo, string icon)
        {
            var project = RavenSession.Load<Project>(projectId);

            if (project.Image != null)
            {
                if (!string.IsNullOrEmpty(project.Image.Logo))
                    DeleteOldImage(projectId, project.Image.Logo);

                if (!string.IsNullOrEmpty(project.Image.Icon))
                    DeleteOldImage(projectId, project.Image.Icon);
            }

            project.Image = new Project.ProjectLogo
                                {
                                    Logo = logo,
                                    Icon = icon
                                };
            RavenSession.SaveChanges();

            return Json("");
        }

        private void DeleteOldImage(string projectId, string imageName)
        {
            var file = Server.MapPath(imageName);

            if (!System.IO.File.Exists(file))
                return;

            System.IO.File.Delete(file);
        }

        [HttpPost]
        public JsonResult RemoveLogo(string projectId, string url)
        {
            var project = RavenSession.Load<Project>(projectId);
            //project.Logo = string.Empty;
            RavenSession.SaveChanges();

            var id = StringHelper.GetProjectId(projectId);



            var uploadUrl = Server.MapPath("/Uploads") + "\\";

            var logoUrl = uploadUrl + "logo_";
            //var logo = ImageUtils.GetImage(id, logoUrl + id);
            //if (!string.IsNullOrEmpty(logo))
            //    System.IO.File.Delete(logoUrl + logo);

            var iconUrl = uploadUrl + "icon_";
            //var icon = ImageUtils.GetImage(id, iconUrl + id);
            //if (!string.IsNullOrEmpty(icon))
            //    System.IO.File.Delete(iconUrl + icon);

            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Upload(HttpPostedFileBase uploadedFile, string projectId)
        {
            // Validate the uploaded file
            if (uploadedFile == null || (uploadedFile.ContentType != "image/jpeg" && uploadedFile.ContentType != "image/png"))
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



            var path = Server.MapPath("/Uploads") + "\\" + projectId.Replace("/", "");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + "\\";

            var completePath = path + uploadedFile.FileName;
            var fileExtension = Path.GetExtension(completePath);
            var newFileName = StringHelper.GetProjectId(projectId) + fileExtension;
            completePath = path + newFileName;
            uploadedFile.SaveAs(completePath);

            var logo = Guid.NewGuid() + fileExtension;
            var icon = Guid.NewGuid() + fileExtension;


            Resize(completePath, path, logo);
            Resize(completePath, path, icon, 24, 24, completePath);

            // Return success code
            return Json(new
            {
                statusCode = 200,
                status = "Image uploaded.",
                logo = logo,
                icon = icon
            }, "text/html");
        }


        [HttpPost]
        public JsonResult UploadProjectImage(HttpPostedFileBase uploadedFile, string projectId)
        {
            //Validate the uploaded file
            if (uploadedFile == null || (uploadedFile.ContentType != "image/jpeg" && uploadedFile.ContentType != "image/png"))
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



            var path = Server.MapPath("/Uploads") + "\\" + projectId.Replace("/", "");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + "\\";

            var completePath = path + uploadedFile.FileName;
            var fileExtension = Path.GetExtension(completePath);
            var newFileName = Guid.NewGuid() + fileExtension;
            completePath = path + newFileName;
            uploadedFile.SaveAs(completePath);

            var image = newFileName;
            var thumb = "thumb_" + image;


            var projectImage = new Project.ProjectImage
                            {
                                Thumbnail = string.Format("/Uploads/{0}/{1}", projectId.Replace("/", ""), thumb),
                                Url = string.Format("/Uploads/{0}/{1}", projectId.Replace("/", ""), Url)
                            };

            ResizeImage(completePath, path, thumb);

            var project = RavenSession.Load<Project>(projectId);

            project.Images.Add(projectImage);
            RavenSession.SaveChanges();

            // Return success code
            return Json(new
            {
                success = true,
                statusCode = 200,
                image = projectImage,
                status = "Image uploaded.",

            }, "text/html");
        }


        private void Resize(String image, string location, string fileName, int height = 104, int width = 104, string original = null)
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

        private void ResizeImage(String image, string location, string fileName)
        {
            var width = 250;
            var height = 150;
            var srcImage = Image.FromFile(image);

            if (srcImage.Height > srcImage.Width)
            {
                width = 150;
                height = 250;
            }

            srcImage.Dispose();

            Resize(image, location, fileName, height, width);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;
using ProjectZ.Web.ViewModels;

namespace ProjectZ.Web.Controllers
{
    public class ReleaseController : RavenController
    {
        //
        // GET: /Release/
        public ActionResult Index()
        {
            var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);
            var releases = RavenSession.Query<Release>().Where(x => x.ProjectId == project.Id).OrderByDescending(x => x.Created).ToList();
            var issues = RavenSession.Query<Issue>().Count(x => x.ProjectId == project.Id);
            return View(new ReleaseViewModel { NumberOfIssues = issues, Project = project, Releases = releases });
        }

        //
        // GET: /Release/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Release/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Release/Create
        [HttpPost]
        public ActionResult Create(Release release)
        {
            try
            {
                var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);
                if (project == null)
                    return View();

                release.ProjectId = project.Id;
                release.Created = DateTime.Now;
                RavenSession.Store(release);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Release/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Release/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Release release)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Release/Delete/5
        public ActionResult Delete(string projectName, int id)
        {
            return View();
        }

        //
        // POST: /Release/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {

                var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);

                if (project == null)
                    return View();


                if (project.Admins.Any(x => x.Id == CurrentUser.Id))
                {
                    var release = RavenSession.Load<Release>(id);
                    RavenSession.Delete(release);
                }


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

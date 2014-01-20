using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.Controllers
{
    public class IssueController : RavenController
    {
        //
        // GET: /Issue/
        public ActionResult Index()
        {
            var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);
            var issues = RavenSession.Query<Issue>().Where(x => x.ProjectId == project.Id).ToList();
            return View(issues);
        }

        //
        // GET: /Issue/Details/5
        public ActionResult Details(string id)
        {
            var issue = RavenSession.Load<Issue>(id);
            return View(issue);
        }

        //
        // GET: /Issue/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Issue/Create
        [HttpPost]
        public ActionResult Create(Issue issue)
        {
            try
            {
                var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);
                if (project == null)
                    return View();

                issue.ProjectId = project.Id;
                issue.UserId = CurrentUser.Id;
                RavenSession.Store(issue);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Issue/Edit/5
        public ActionResult Edit(string id)
        {


            return View();
        }

        //
        // POST: /Issue/Edit/5
        [HttpPost]
        public ActionResult Edit(string projectName, int id, FormCollection collection)
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
        // POST: /Issue/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {

                var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);

                if (project == null)
                    return View();


                if (project.Admins.Any(x => x.Id == CurrentUser.Id))
                    RavenSession.Delete(id);
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

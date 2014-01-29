using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;
using ProjectZ.Web.ViewModels;

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
            return View(new IssueViewModel { Project = project, Issues = issues, UserId = CurrentUser == null ? string.Empty : CurrentUser.Id });
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
                issue.Votes.Add(new Vote { UserId = CurrentUser.Id });
                RavenSession.Store(issue);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult Upvote(string issueId)
        {
            if (CurrentUser == null)
                return Json(new { success = false, message = "You are not logged in" });

            var issue = RavenSession.Load<Issue>(issueId);

            if (issue == null)
                return Json(new { success = false, message = "You are not admin of this project" });

            if (issue.Votes != null && issue.Votes.Any() && issue.Votes.Select(x => x.UserId).Contains(CurrentUser.Id))
                return Json(new { success = false, message = "You have already voted for this" });

            issue.Votes.Add(new Vote { UserId = CurrentUser.Id });
            RavenSession.SaveChanges();

            return Json(new { success = true, message = string.Empty });
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
        public ActionResult Edit(int id, FormCollection collection)
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
                {
                    var issue = RavenSession.Load<Issue>(id);
                    RavenSession.Delete(issue);
                }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;
using ProjectZ.Web.ViewModels;
using Action = ProjectZ.Web.Models.Action;

namespace ProjectZ.Web.Controllers
{
    public class IssueController : RavenController
    {
        //
        // GET: /Issue/

        //
        // GET: /Issue/Details/5
        public ActionResult Details(string id)
        {
            var issue = RavenSession.Load<Issue>(id);
            return View(issue);
        }

        [HttpPost]
        public JsonResult Comment(string comment, string projectId, int issueId)
        {
            var project = RavenSession.Load<Project>(projectId);
            var issue = project.Issues.FirstOrDefault(x => x.Id == issueId);

            var issueComment = new IssueComment
                                   {
                                       Id = issue.Comments.Count() + 1,
                                       Comment = comment,
                                       Posted = DateTime.Now,
                                       User = new IssueCommentUser(CurrentUser)
                                   };

            issue.Comments.Add(issueComment);

            var eventAction = new EventAction()
            {
                Action = Action.Comment,
                Created = DateTime.Now,
                ProjectId = projectId,
                Title = comment,
                ProjectName = project.Name,
                Url = string.Format("/{0}/{1}/issues/{2}#{3}", projectId, project.Slug, issue.Id, issueComment.Id),
                User = new EventActionUser(CurrentUser),
                Reference = new EventActionReference
                                {
                                    Id = issue.Id.ToString(),
                                    Name = issue.Title
                                }
            };
            RavenSession.Store(eventAction);

            RavenSession.SaveChanges();
            return Json("");
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
        public JsonResult Create(Issue issue, string projectId)
        {
            try
            {
                var project = RavenSession.Load<Project>(projectId);
                if (project == null)
                    return Json(new { success = false, message = "No project found" });

                if (CurrentUser == null)
                    return Json(new { success = false, message = "You are not logged in" });

                issue.User = new IssueUser(CurrentUser);
                issue.Votes.Add(new Vote { UserId = CurrentUser.Id });
                issue.Posted = DateTime.Now;
                issue.Id = project.Issues.Count() + 1;
                project.Issues.Add(issue);

                var eventAction = new EventAction()
                               {
                                   Action = issue.IssueType == IssueType.Feature ? Action.Feature : Action.Bug,
                                   Created = DateTime.Now,
                                   ProjectId = projectId,
                                   Title = issue.Title,
                                   ProjectName = project.Name,
                                   Url = string.Format("/{0}/{1}/issues/{2}", projectId, project.Slug, issue.Id),
                                   User = new EventActionUser(CurrentUser)
                               };
                RavenSession.Store(eventAction);
                RavenSession.SaveChanges();


                return Json(new { success = true, message = issue.Id });
            }
            catch
            {
                return Json(new { success = false, message = "Error" });

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
        //[HttpPost]
        //public ActionResult Delete(string id)
        //{
        //    try
        //    {

        //        var project = RavenSession.Query<Project>().FirstOrDefault(x => x.Name == Subdomain);

        //        if (project == null)
        //            return View();


        //        if (project.Admins.Any(x => x.Id == CurrentUser.Id))
        //        {
        //            var issue = RavenSession.Load<Issue>(id);
        //            RavenSession.Delete(issue);
        //        }
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}

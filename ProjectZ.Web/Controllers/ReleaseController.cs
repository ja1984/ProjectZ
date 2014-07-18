using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using ProjectZ.Web.Models;
using ProjectZ.Web.ViewModels;
using Action = ProjectZ.Web.Models.Action;

namespace ProjectZ.Web.Controllers
{
    public class ReleaseController : RavenController
    {
        [HttpPost]
        public JsonResult Create(CreateReleaseModel release)
        {
            var project = RavenSession.Load<Project>(release.ProjectId);
            var issues = new List<Issue>();
            if (release.SolvedIssues != null)
            {
                issues.AddRange(project.Issues.Where(x => release.SolvedIssues.Contains(x.Id)).ToList());

                foreach (var issue in issues)
                {
                    issue.Solved = true;
                }
            }

            var newRelease = new Release
            {
                Created = DateTime.Now,
                Description = release.Description,
                Issues = issues,
                ProjectId = release.ProjectId,
                Title = release.Title,
                Version = release.Version
            };

            var eventAction = new EventAction()
            {
                Action = Action.Release,
                Created = DateTime.Now,
                ProjectId = release.ProjectId,
                Title = release.Version,
                ProjectName = project.Name,
                Url = string.Format("/{0}/{1}/releases/{2}", release.ProjectId, project.Slug, release.Id),
                User = new EventActionUser(CurrentUser)
            };

            RavenSession.Store(eventAction);
            RavenSession.Store(newRelease);
            RavenSession.SaveChanges();

            return Json(new { Success = true });
        }

    }
}

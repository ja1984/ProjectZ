using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;
using Raven.Client.Linq;

namespace ProjectZ.Web.Controllers
{
    public class HomeController : RavenController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (CurrentUser != null)
            {
                return RedirectToAction("HomeFeed");
            }

            return View();
        }

        [Authorize]
        public ActionResult HomeFeed()
        {
            var projects = CurrentUser.Follows.Select(x => x.Id).ToList();

            var events = new List<EventAction>();
            events.AddRange(RavenSession.Query<EventAction>().Where(x => x.ProjectId.In(projects)));

            return View(events);
        }

    }
}

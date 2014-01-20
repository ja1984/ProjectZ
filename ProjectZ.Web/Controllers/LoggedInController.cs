using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectZ.Web.Controllers
{
    public class LoggedInController : RavenController
    {
        //
        // GET: /LoggedIn/

        public ActionResult Index()
        {

            if (CurrentUser == null)
                return PartialView("NotLoggedIn");

            return PartialView(CurrentUser);
        }

    }
}

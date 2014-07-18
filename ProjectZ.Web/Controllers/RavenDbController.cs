using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;
using Raven.Client;
using Raven.Client.Document;

namespace ProjectZ.Web.Controllers
{
    public abstract class RavenController : Controller
    {
        public IDocumentSession RavenSession { get; protected set; }

        public User CurrentUser { get; set; }

        protected RavenController()
        {
            //DocumentStore.Conventions.SaveEnumsAsIntegers = true;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = MvcApplication.Store.OpenSession();

            var userFromAuthCookie = User;

            if (userFromAuthCookie != null && userFromAuthCookie.Identity.IsAuthenticated)
            {
                using (RavenSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
                {
                    CurrentUser = GetUser(userFromAuthCookie);
                }
                //If hes not found in the cache we try against the database. Cause was a problem with new user that didnt get recgonized.
                if (CurrentUser == null)
                {
                    CurrentUser = GetUser(userFromAuthCookie);
                }
            }

        }

        private User GetUser(IPrincipal userFromAuthCookie)
        {
            var user = RavenSession.Load<User>(userFromAuthCookie.Identity.Name);
            return user;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            using (RavenSession)
            {
                if (filterContext.Exception != null)
                    return;

                if (RavenSession != null)
                    RavenSession.SaveChanges();
            }
        }

        protected HttpStatusCodeResult HttpNotModified()
        {
            return new HttpStatusCodeResult(304);
        }


        protected new JsonResult Json(object data)
        {
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }
    }

}

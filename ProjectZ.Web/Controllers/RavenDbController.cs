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
        protected RavenController()
        {
            DocumentStore.Conventions.SaveEnumsAsIntegers = true;            
        }

        public User CurrentUser { get; set; }
        public static IDocumentStore DocumentStore
        {
            get
            {
                return new DocumentStore
                {
                    Url = "https://ec2-eu5.cloudbird.net/databases/d56aa754-cac6-4505-9871-5693754df001.projectz",
                    ApiKey = "88d52e06-c340-4fa1-95e9-2bd502272a03"
                }.Initialize();
            }
        }
         
        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = DocumentStore.OpenSession();
            DocumentStore.Conventions.SaveEnumsAsIntegers = true;
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
